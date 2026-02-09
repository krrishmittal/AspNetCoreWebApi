using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using GoogleLogin.Models.Api;
using GoogleLogin.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using GoogleLogin.Repository;

namespace GoogleLogin.Controllers 
{
    public class RegisterController : Controller
    {
        private readonly JwtService _jwtService;
        private readonly ILogger<RegisterController> _logger;
        private readonly IUserRepo _userRepo;

        public RegisterController(JwtService jwtService, ILogger<RegisterController> logger, IUserRepo userRepo)
        {
            _jwtService = jwtService;
            _logger = logger;
            _userRepo = userRepo;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // Clear any lingering TempData messages
            TempData.Remove("SuccessMessage");
            TempData.Remove("ErrorMessage");
            
            return View("Register");
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromForm] RegisterRequest request)
        {
            var (success, message) = await _jwtService.Register(request);

            if (!success)
            {
                TempData["ErrorMessage"] = message;
                return View("Register");
            }

            _logger.LogInformation("User {Username} registered successfully", request.Username);

            // Auto-login logic: Get the newly registered user
            var user = _userRepo.GetByUsername(request.Username);
            if (user == null)
            {
                _logger.LogError("User {Username} not found after registration", request.Username);
                TempData["ErrorMessage"] = "Registration successful but login failed. Please login manually.";
                return RedirectToAction("Index", "Login");
            }
            // Create claims for cookie authentication
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

            // Sign in the user with cookie authentication
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            _logger.LogInformation("User {Username} automatically logged in after registration", request.Username);
            
            // Don't set TempData message since user is being redirected to Home and is already logged in
            // The home page will show their information directly
            return RedirectToAction("Index", "Home");
        }
    }
}
