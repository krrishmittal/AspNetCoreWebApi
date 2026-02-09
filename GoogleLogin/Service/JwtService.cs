using GoogleLogin.Handlers;
using GoogleLogin.Models;
using GoogleLogin.Models.Api;
using GoogleLogin.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GoogleLogin.Service
{
    public class JwtService
    {
        private IConfiguration _configuration;
        private readonly AppdbContext db;
        private readonly ILogger<JwtService> _logger;
        private readonly IUserRepo _userRepo;
        private readonly EncryptionService _encryptionService;

        public JwtService(AppdbContext db, IConfiguration configuration, ILogger<JwtService> logger, IUserRepo userRepo, EncryptionService encryptionService)
        {
            this.db = db;
            _configuration = configuration;
            _logger = logger;
            _userRepo = userRepo;
            _encryptionService = encryptionService;
        }

        public async Task<AuthenticationResult> Authenticate(LoginRequest request)
        {
            _logger.LogInformation("Authentication attempt started for username: {Username}", request.Username);

            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                _logger.LogWarning("Authentication failed: Username or password is empty");
                return AuthenticationResult.Failed("Invalid username or password");
            }

            // Get user by username
            var user = await db.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            
            if (user == null)
            {
                _logger.LogWarning("Authentication failed: User '{Username}' not found", request.Username);
                return AuthenticationResult.Failed("Invalid username or password");
            }

            // Check if user is deleted
            if (user.Isdeleted)
            {
                _logger.LogWarning("Authentication failed: User '{Username}' is deleted", request.Username);
                return AuthenticationResult.Failed("User does not exist");
            }

            // Check if user registered via OAuth (no password set)
            if (string.IsNullOrEmpty(user.Password))
            {
                _logger.LogWarning("Authentication failed: User '{Username}' registered via OAuth and cannot use password login", request.Username);
                return AuthenticationResult.Failed("This account was created using Google Sign-In. Please use 'Sign in with Google' button to login.");
            }

            _logger.LogDebug("User '{Username}' found in database, verifying password", request.Username);

            // Verify the password using bcrypt
            if (!PasswordHash.VerifyPass(request.Password, user.Password))
            {
                _logger.LogWarning("Authentication failed: Invalid password for user '{Username}'", request.Username);
                return AuthenticationResult.Failed("Invalid username or password");
            }

            _logger.LogInformation("Password verified successfully for user '{Username}', generating JWT token", request.Username);

            // Generate token for regular login
            var accessToken = GenerateJwtToken(user);

            var loginResponse = new LoginResponse
            {
                AccessToken = accessToken,
                Username = request.Username,
                ExpiresIn = _configuration.GetValue<int>("Jwt:TokenValidityInMinutes") * 60
            };

            return AuthenticationResult.Successful(loginResponse);
        }

        // New method for Google OAuth users
        public string GenerateTokenForOAuthUser(User user)
        {
            _logger.LogInformation("Generating JWT token for OAuth user: {Username}", user.Username);
            return GenerateJwtToken(user);
        }

        // Consolidated token generation method
        private string GenerateJwtToken(User user)
        {
            var issuer = _configuration["Jwt:ValidIssuer"];
            var audience = _configuration["Jwt:ValidAudience"];
            var key = _configuration["Jwt:Secret"];
            var tokenValidMins = _configuration.GetValue<int>("Jwt:TokenValidityInMinutes");
            var tokenExpiry = DateTime.UtcNow.AddMinutes(tokenValidMins);

            // Encrypt sensitive claim values
            var encryptedUsername = _encryptionService.Encrypt(user.Username);
            var encryptedRole = _encryptionService.Encrypt(user.Role);
            var encryptedUserId = _encryptionService.Encrypt(user.Id.ToString());
            var encryptedEmail = _encryptionService.Encrypt(user.Email.ToString());

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Name, encryptedUsername),
                    new Claim(ClaimTypes.Role, encryptedRole),
                    new Claim(ClaimTypes.Email, encryptedEmail),
                    new Claim(JwtRegisteredClaimNames.Sub, encryptedUserId),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = tokenExpiry,
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), 
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(token);

            _logger.LogInformation("JWT token with encrypted claims generated successfully for user '{Username}', expires at {ExpiryTime}", user.Username, tokenExpiry);
            Console.WriteLine($"access token: {accessToken}");
            return accessToken;
        }

        public async Task<(bool Success, string Message)> Register(RegisterRequest request)
        {
            _logger.LogInformation(
                "Registration attempt started for username: {Username}",
                request.Username
            );

            if (string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                _logger.LogWarning(
                    "Registration failed: Missing username or password"
                );
                return (false, "Username and password are required");
            }

            var existingUser = _userRepo.GetByUsername(request.Username);
            if (existingUser != null)
            {
                _logger.LogWarning(
                    "Registration failed: Username already exists - {Username}",
                    request.Username
                );
                return (false, "Username already exists");
            }

            var isFirstUser = !await db.Users.AnyAsync();
            var userRole = isFirstUser ? "Admin" : "User";

            var user = new User
            {
                Username = request.Username,
                Password = PasswordHash.HashPass(request.Password),
                Email = request.Email,
                Role = userRole,
                Isdeleted = false
            };

            _userRepo.AddUser(user);
            _userRepo.SaveChanges();

            _logger.LogInformation(
                "User registered successfully with username: {Username}, Role: {Role}",
                request.Username,
                userRole
            );

            return (true, $"User registered successfully with role: {userRole}");
        }

        public async Task<User?> UpdateUserRole(int userId, string newRole)
        {
            var user = await db.Users.FindAsync(userId);
            if (user == null)
            {
                return null;
            }

            user.Role = newRole;
            await db.SaveChangesAsync();

            _logger.LogInformation("User {Username} role updated to {Role}", user.Username, newRole);
            return user;
        }
    }
}
