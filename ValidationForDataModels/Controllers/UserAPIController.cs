using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ValidationForDataModels.Service;
using Serilog;
using ValidationForDataModels.DTO;
namespace ValidationForDataModels.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAPIController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly ILogger<UserAPIController> logger;
        public UserAPIController(IUserService userService, ILogger<UserAPIController> logger)
        {
            this.userService = userService;
            this.logger = logger;
        }
        [HttpGet]
        public IActionResult GetUsers()
        {
            try
            {
                logger.LogInformation("GetUsers called");
                var users = userService.GetUser();
                logger.LogInformation("GetUsers successful, retrieved {Count} users", users.Count());
                return Ok(users);
            }
            catch (Exception ex)
            {
                Log.Error("Error in GetUsers: {Message}", ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("{id:int}")]
        public IActionResult GetUserById(int id)
        {
            try
            {
                logger.LogInformation("GetUserById called with id: {Id}", id);
                var user = userService.GetUserById(id);
                if (user == null)
                {
                    logger.LogWarning("User with id: {Id} not found", id);
                    return NotFound();
                }
                logger.LogInformation("GetUserById successful for id: {Id}", id);
                return Ok(user);
            }
            catch (Exception ex)
            {
                Log.Error("Error in GetUserById: {Message}", ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPost]
        public IActionResult CreateUser(AddUser user) 
        {
            try
            {
                logger.LogInformation("CreateUser called");
                var createdUser = userService.AddUser(user);
                logger.LogInformation("CreateUser successful with id: {Id}", createdUser.Id);
                return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
            }
            catch (Exception ex)
            {
                Log.Error("Error in CreateUser: {Message}", ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateUser(int id, UpdateUser userDto)
        {
            try
            {
                logger.LogInformation("UpdateUser called for id: {Id}", id);
                var updatedUser = userService.UpdateUser(userDto, id);
                logger.LogInformation("UpdateUser successful for id: {Id}", id);
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                Log.Error("Error in UpdateUser: {Message}", ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpDelete("{id:int}")]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                logger.LogInformation("DeleteUser called for id: {Id}", id);
                var result = userService.DeleteUser(id);
                if (!result)
                {
                    logger.LogWarning("User with id: {Id} not found for deletion", id);
                    return NotFound();
                }
                logger.LogInformation("DeleteUser successful for id: {Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                Log.Error("Error in DeleteUser: {Message}", ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
