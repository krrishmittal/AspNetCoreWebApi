using CRUD.DTO;
using CRUD.Models;

namespace CRUD.Services
{
    public interface IEmployeeService
    {
        List<EmployeeRead> GetAllEmployees();
        EmployeeInfo AddEmployee(EmployeeCreate empDto);
        EmployeeInfo UpdateEmployee(EmployeeUpdate empDto, int id);
        bool DeleteEmployee(int id);
    }
}