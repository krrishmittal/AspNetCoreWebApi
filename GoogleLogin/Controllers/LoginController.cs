using GoogleLogin.Service;
using GoogleLogin.Models.Api;
using GoogleLogin.Repository;
using GoogleLogin.Handlers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GoogleLogin.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace GoogleLogin.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUserRepo _userRepo;
        private readonly JwtService _jwtService;
        private readonly ILogger<LoginController> _logger;

        public LoginController(IUserRepo userRepo, JwtService jwtService, ILogger<LoginController> logger)
        {
            _userRepo = userRepo;
            _jwtService = jwtService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View("Login");
        }

        // Regular username/password login
        [HttpPost("/login")]
        public async Task<IActionResult> Login([FromForm] LoginRequest request)
        {
            var result = await _jwtService.Authenticate(request);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
                return RedirectToAction("Index");
            }

            _logger.LogInformation("User {Username} logged in successfully", request.Username);

            // Get user from database
            var user = _userRepo.GetByUsername(request.Username ?? string.Empty);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found";
                return RedirectToAction("Index");
            }

            // Sign in with Cookie authentication for MVC views
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return RedirectToAction("Index", "Home");
        }

        // Google OAuth login
        [HttpGet("/login/signin-google")]
        public IActionResult GoogleLogin()
        {
            return Challenge(
                new AuthenticationProperties
                {
                    RedirectUri = "/auth/callback"
                },
                GoogleDefaults.AuthenticationScheme
            );
        }

        // Google OAuth callback
        [HttpGet("/auth/callback")]
        public async Task<IActionResult> Callback()
        {
            // First, authenticate with Google to get their claims
            var authenticateResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            
            if (!authenticateResult.Succeeded)
            {
                _logger.LogWarning("Google authentication failed");
                TempData["ErrorMessage"] = "Google authentication failed";
                return RedirectToAction("Index");
            }

            var email = authenticateResult.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = authenticateResult.Principal.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                _logger.LogWarning("Email not found in Google claims");
                TempData["ErrorMessage"] = "Unable to retrieve email from Google";
                return RedirectToAction("Index");
            }
            var existingUser = _userRepo.GetByEmail(email);

            if (existingUser == null)
            {
                // Check if this is the first user in the system
                var isFirstUser = !_userRepo.GetUser().Any();
                var userRole = isFirstUser ? "Admin" : "User";

                // Create new user
                var newUser = new User
                {
                    Username = name ?? email.Split('@')[0],
                    Email = email,
                    Password = "",
                    Role = userRole,
                    Isdeleted = false
                };
                _userRepo.AddUser(newUser);
                _userRepo.SaveChanges();

                existingUser = newUser;
                _logger.LogInformation("New user created via Google OAuth: {Email} with role: {Role}", email, userRole);
            }
            else if (existingUser.Isdeleted == true)
            {
                existingUser.Isdeleted = false;
                _userRepo.SaveChanges();
                _logger.LogInformation("Reactivated user: {Email}", email);
            }

            // Generate JWT token and store in session
            var jwtToken = _jwtService.GenerateTokenForOAuthUser(existingUser);
            HttpContext.Session.SetString("JwtToken", jwtToken);

            // Create claims with database user information
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, existingUser.Id.ToString()),
                new Claim(ClaimTypes.Name, existingUser.Username),
                new Claim(ClaimTypes.Email, existingUser.Email),
                new Claim(ClaimTypes.Role, existingUser.Role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
            };

            // Sign in with cookie authentication using database user claims
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            _logger.LogInformation("User logged in successfully via Google: {Email}, Role: {Role}, ID: {Id}", 
                email, existingUser.Role, existingUser.Id);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet("/logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
