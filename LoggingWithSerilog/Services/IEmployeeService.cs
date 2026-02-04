using LoggingWithSerilog.DTO;
using LoggingWithSerilog.Models;

namespace LoggingWithSerilog.Services
{
    public interface IEmployeeService
    {
        Task<PaginatedResult<EmployeeRead>> GetAllEmployeesAsync(
            string? filterOn = null,
            string? filterQuery = null,
            string? sortBy = null,
            bool isAscending = true,
            int pageNumber = 1,
            int pageSize = 10);
        EmployeeInfo AddEmployee(EmployeeCreate empDto);
        EmployeeInfo UpdateEmployee(EmployeeUpdate empDto, int id);
        bool DeleteEmployee(int id);
    }
}