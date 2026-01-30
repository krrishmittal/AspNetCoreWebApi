using EmployeeCRUD.Models;
namespace EmployeeCRUD.Repositories
{
    public interface IEmployeeRepository
    {
        IEnumerable<EmployeeInfo> GetEmployee();
        EmployeeInfo GetEmployeeById(int id);
        void AddEmployee(EmployeeInfo emp);
        void UpdateEmployee(EmployeeInfo emp);
        void DeleteEmployee(int id);
        void Save();
    }
}
