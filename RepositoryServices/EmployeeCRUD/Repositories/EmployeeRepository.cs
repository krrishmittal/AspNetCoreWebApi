using EmployeeCRUD.Models;

namespace EmployeeCRUD.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly DumContext db;

        public EmployeeRepository(DumContext db)
        {
            this.db = db;
        }

        public IEnumerable<EmployeeInfo> GetEmployee()
        {
            return db.EmployeeInfos.Where(e => !e.Isdeleted).ToList();
        }
        public EmployeeInfo GetEmployeeById(int id)
        {
            return db.EmployeeInfos.Find(id);
        }
        public void AddEmployee(EmployeeInfo employee)
        {
            db.EmployeeInfos.Add(employee);
        }

        public void UpdateEmployee(EmployeeInfo employee)
        {
            db.EmployeeInfos.Update(employee);
        }

        public void DeleteEmployee(int id)
        {
            var employee = GetEmployeeById(id);
            if (employee != null)
            {
                employee.Isdeleted = true;
            }
        }

        public void Save()
        {
            db.SaveChanges();
        }
    }
}