using LoggingWithSerilog.Models;

namespace LoggingWithSerilog.Repositories
{
    public interface IEmployeeRepository
    {
        IQueryable<EmployeeInfo> GetEmployeesQuery();
        EmployeeInfo GetEmployeeById(int id);
        void AddEmployee(EmployeeInfo emp);
        void UpdateEmployee(EmployeeInfo emp);
        void DeleteEmployee(int id);
        void Save();
    }
}
