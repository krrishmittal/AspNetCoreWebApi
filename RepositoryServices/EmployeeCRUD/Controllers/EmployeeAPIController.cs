using EmployeeCRUD.DTO;
using EmployeeCRUD.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeCRUD.Controllers
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

        [HttpGet]
        public IActionResult Get()
        {
            var empDto = employeeService.GetAllEmployees();
            return Ok(empDto);
        }

        [HttpPost]
        public IActionResult AddEmployee(EmployeeCreate emp)
        {
            var empEntity = employeeService.AddEmployee(emp);
            return Ok(empEntity);
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