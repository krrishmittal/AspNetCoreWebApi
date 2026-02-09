using JwtAuth.Service;
using JwtAuth.Models.Api;
using JwtAuth.Repository;
using JwtAuth.Handlers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using JwtAuth.Models;

namespace JwtAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly JwtService _js;
        private readonly ILogger<AccountController> _logger;

        public AccountController(JwtService js, ILogger<AccountController> logger)
        {
            _js = js;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterRequest request)
        {
            var (success, message) = await _js.Register(request);
            
            if (!success)
            {
                return BadRequest(message);
            }

            return Ok(message);
        }   

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
        {
            var result = await _js.Authenticate(request);
            
            if (!result.Success)
            {
                _logger.LogWarning("Login failed for username: {Username}. Reason: {Reason}", 
                    request.Username, result.ErrorMessage);
                return Unauthorized(new { message = result.ErrorMessage });
            }
            
            _logger.LogInformation("Login successful for username: {Username}", request.Username);
            return Ok(result.Data);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("promote/{userId}")]
        public async Task<ActionResult> PromoteUser(int userId, [FromBody] string newRole)
        {
            var user = await _js.UpdateUserRole(userId, newRole);
            
            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok($"User role updated to {newRole}");
        }
    }
}
