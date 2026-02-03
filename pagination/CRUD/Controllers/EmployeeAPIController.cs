using CRUD.DTO;
using CRUD.Services;
using Microsoft.AspNetCore.Mvc;

namespace CRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeAPIController : ControllerBase
    {
        private readonly IEmployeeService employeeService;

        public EmployeeAPIController(IEmployeeService employeeService)
        {
            this.employeeService = employeeService;
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
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var result = await employeeService.GetAllEmployeesAsync(
                filterOn,
                filterQuery,
                sortBy,
                isAscending ?? true,
                pageNumber,
                pageSize);

            return Ok(result);
        }

        [HttpPost]
        public IActionResult AddEmployee(EmployeeCreate emp)
        {
            return Ok(employeeService.AddEmployee(emp));
        }

        [HttpPut("{id}")]
        public IActionResult UpdateEmployee(EmployeeUpdate empDto, int id)
        {
            var emp = employeeService.UpdateEmployee(empDto, id);
            if (emp == null)
            {
                return NotFound($"Employee with Id {id} does not exist");
            }
            return Ok(emp);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteEmployee(int id)
        {
            var success = employeeService.DeleteEmployee(id);
            if (!success)
            {
                return NotFound("Employee doesnot exists");
            }
            return Ok("Employee deleted successfully");
        }
    }
}