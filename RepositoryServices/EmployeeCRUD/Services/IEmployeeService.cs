using EmployeeCRUD.DTO;
using EmployeeCRUD.Models;

namespace EmployeeCRUD.Services
{
    public interface IEmployeeService
    {
        List<EmployeeRead> GetAllEmployees();
        EmployeeInfo AddEmployee(EmployeeCreate empDto);
        EmployeeInfo UpdateEmployee(EmployeeUpdate empDto, int id);
        bool DeleteEmployee(int id);
    }
}