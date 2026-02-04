using LoggingWithSerilog.DTO;
using LoggingWithSerilog.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;
//using Serilog;

namespace LoggingWithSerilog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeAPIController : ControllerBase
    {
        private readonly IEmployeeService employeeService;
        private readonly ILogger<EmployeeAPIController> logger;

        public EmployeeAPIController(IEmployeeService employeeService, ILogger<EmployeeAPIController> logger)
        {
            this.employeeService = employeeService;
            this.logger = logger;
        }
        // OLD Controller - simple parameters
        //public IActionResult Get(int pageNumber, int pageSize, string search)
        //{
        //    var employees = employeeService.GetEmployeesPaged(pageNumber, pageSize, search);
        //    return Ok(employees); // Just returns array
        //}
        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] string? filterOn,
            [FromQuery] string? filterQuery,
            [FromQuery] string? sortBy,
            [FromQuery] bool? isAscending,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                //Log.Information("Fetching employees with pagination");
                logger.LogInformation("Fetching employees with PageNumber: {PageNumber}, PageSize: {PageSize}, FilterOn: {FilterOn}, FilterQuery: {FilterQuery}, SortBy: {SortBy}, IsAscending: {IsAscending}",
                    pageNumber, pageSize, filterOn, filterQuery, sortBy, isAscending);
                
                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 10;

                var result = await employeeService.GetAllEmployeesAsync(
                    filterOn,
                    filterQuery,
                    sortBy,
                    isAscending ?? true,
                    pageNumber,
                    pageSize);
                //Log.Information("Employees fetched successfully");
                logger.LogInformation("Successfully fetched {Count} employees", result?.TotalCount ?? 0);
                return Ok(result);
            }
            catch (Exception ex)
            {
                //Log.Fatal(ex, "Error occurred while fetching employees");
                logger.LogError(ex, "Error occurred while fetching employees with PageNumber: {PageNumber}, PageSize: {PageSize}",
                    pageNumber, pageSize);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public IActionResult AddEmployee(EmployeeCreate emp)
        {
            try
            {
                //    Log.Information("Employee Adding");
                //    Log.Information("Employee Added Successfully");
                logger.LogInformation("Adding employee with Name: {FirstName} {LastName}", emp.Fname, emp.Lname);
                
                var result = employeeService.AddEmployee(emp);
                
                logger.LogInformation("Employee added successfully with Id: {EmployeeId}", result.Id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                //Log.Fatal(ex, "Error occurred while adding employee");
                logger.LogError(ex, "Error occurred while adding employee");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateEmployee(EmployeeUpdate empDto, int id)
        {
            try
            {
                //Log.Information("Updating employee details");
                logger.LogInformation("Updating employee with Id: {EmployeeId}", id);
                
                var emp = employeeService.UpdateEmployee(empDto, id);
                if (emp == null)
                {
                    logger.LogWarning("Employee with Id: {EmployeeId} not found", id);
                    return NotFound($"Employee with Id {id} does not exist");
                }
                
                logger.LogInformation("Employee with Id: {EmployeeId} updated successfully", id);
                return Ok(emp);
            }
            catch (Exception ex)
            {
                //Log.Fatal(ex, "Error occurred while updating employee");
                logger.LogError(ex, "Error occurred while updating employee with Id: {EmployeeId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteEmployee(int id)
        {
            try
            {
                //Log.Information("Deleting employee");
                logger.LogInformation("Attempting to delete employee with Id: {EmployeeId}", id);
                
                var success = employeeService.DeleteEmployee(id);
                if (!success)
                {
                    logger.LogWarning("Employee with Id: {EmployeeId} not found for deletion", id);
                    return NotFound($"Employee with Id {id} does not exist");
                }
                
                logger.LogInformation("Employee with Id: {EmployeeId} deleted successfully", id);
                return Ok("Employee deleted successfully");
            }
            catch (Exception ex)
            {
                //Log.Fatal(ex, "Error occurred while deleting employee");
                logger.LogError(ex, "Error occurred while deleting employee with Id: {EmployeeId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}