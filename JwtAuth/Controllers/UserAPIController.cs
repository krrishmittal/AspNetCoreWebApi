using JwtAuth.DTO;
using JwtAuth.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Security.Claims;

namespace JwtAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class UserAPIController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly ILogger<UserAPIController> logger;
        private readonly EncryptionService _encryptionService;

        public UserAPIController(IUserService userService, ILogger<UserAPIController> logger, EncryptionService encryptionService)
        {
            this.userService = userService;
            this.logger = logger;
            _encryptionService = encryptionService;
        }
        
        [HttpGet]
        [Authorize(Roles = "Admin")] 
        public IActionResult GetUsers()
        {
            try
            {
                logger.LogInformation("GetUsers called");
                var users = userService.GetUser();
                logger.LogInformation("GetUsers successful, retrieved {Count} users", users.Count());
                return Ok(users);
            }
            catch (UnauthorizedAccessException ex)
            {
                logger.LogWarning(ex, "Unauthorized access in GetUsers");
                return Unauthorized(new { message = "You are not authorized to access this resource" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in GetUsers");
                return StatusCode(500, new { message = "An unexpected error occurred" });
            }
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            try
            {
                var userId = GetCurrentUserId();
                logger.LogInformation("GetCurrentUser called for user id: {UserId}", userId);
                
                var user = userService.GetUserById(userId);
                if (user == null)
                {
                    logger.LogWarning("Current user with id: {UserId} not found", userId);
                    return NotFound(new { message = "User not found" });
                }
                
                logger.LogInformation("GetCurrentUser successful for id: {UserId}", userId);
                return Ok(user);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in GetCurrentUser");
                return StatusCode(500, new { message = "An unexpected error occurred" });
            }
        }
        
        [HttpGet("{id:int}")]
        [Authorize]
        public IActionResult GetUserById(int id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var isAdmin = User.IsInRole("Admin");

                if (!isAdmin && currentUserId != id)
                {
                    logger.LogWarning("User {CurrentUserId} attempted to access user {RequestedId}", currentUserId, id);
                    return Forbid();
                }

                logger.LogInformation("GetUserById called with id: {Id}", id);
                var user = userService.GetUserById(id);
                if (user == null)
                {
                    logger.LogWarning("User with id: {Id} not found", id);
                    return NotFound(new { message = $"User with id {id} not found" });
                }
                logger.LogInformation("GetUserById successful for id: {Id}", id);
                return Ok(user);
            }
            catch (UnauthorizedAccessException ex)
            {
                logger.LogWarning(ex, "Unauthorized access in GetUserById for id: {Id}", id);
                return Unauthorized(new { message = "You are not authorized to access this resource" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in GetUserById for id: {Id}", id);
                return StatusCode(500, new { message = "An unexpected error occurred" });
            }
        }
        
        [HttpPost]
        [Authorize(Roles = "Admin")] 
        public IActionResult CreateUser(AddUser user)
        {
            try
            {
                logger.LogInformation("CreateUser called");
                var createdUser = userService.AddUser(user);
                logger.LogInformation("CreateUser successful with id: {Id}", createdUser.Id);
                return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
            }
            catch (ArgumentException ex)
            {
                logger.LogWarning(ex, "Invalid argument in CreateUser");
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                logger.LogWarning(ex, "Invalid operation in CreateUser");
                return Conflict(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                logger.LogWarning(ex, "Unauthorized access in CreateUser");
                return Unauthorized(new { message = "You are not authorized to perform this action" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in CreateUser");
                return StatusCode(500, new { message = "An unexpected error occurred" });
            }
        }

        [HttpPut("me")]
        [Authorize]
        public IActionResult UpdateCurrentUser(UpdateUser userDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                logger.LogInformation("UpdateCurrentUser called for user id: {UserId}", userId);
                
                var updatedUser = userService.UpdateUser(userDto, userId);
                if (updatedUser == null)
                {
                    logger.LogWarning("User with id: {UserId} not found for update", userId);
                    return NotFound(new { message = "User not found" });
                }
                
                logger.LogInformation("UpdateCurrentUser successful for id: {UserId}", userId);
                return Ok(updatedUser);
            }
            catch (ArgumentException ex)
            {
                logger.LogWarning(ex, "Invalid argument in UpdateCurrentUser");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in UpdateCurrentUser");
                return StatusCode(500, new { message = "An unexpected error occurred" });
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                logger.LogInformation("DeleteUser called for id: {Id}", id);
                var result = userService.DeleteUser(id);
                if (!result)
                {
                    logger.LogWarning("User with id: {Id} not found for deletion", id);
                    return NotFound(new { message = $"User with id {id} not found" });
                }
                logger.LogInformation("DeleteUser successful for id: {Id}", id);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                logger.LogWarning(ex, "Unauthorized access in DeleteUser for id: {Id}", id);
                return Unauthorized(new { message = "You are not authorized to perform this action" });
            }
            catch (InvalidOperationException ex)
            {
                logger.LogWarning(ex, "Cannot delete user with id: {Id}", id);
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in DeleteUser for id: {Id}", id);
                return StatusCode(500, new { message = "An unexpected error occurred" });
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) 
                           ?? User.FindFirst("sub");
            
            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException("User ID not found in token");
            }

            var decryptedUserId = _encryptionService.Decrypt(userIdClaim.Value);
            
            if (!int.TryParse(decryptedUserId, out int userId))
            {
                throw new UnauthorizedAccessException("Invalid User ID in token");
            }
            
            return userId;
        }
    }
}
