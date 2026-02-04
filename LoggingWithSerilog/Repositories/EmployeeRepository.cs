using LoggingWithSerilog.Models;

namespace LoggingWithSerilog.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly DumContext db;

        public EmployeeRepository(DumContext db)
        {
            this.db = db;
        }

        // OLD Repository - returned executed list
        //public IEnumerable<EmployeeInfo> GetEmployeePaged(int pageNumber, int pageSize, string search)
        //{
        //    var query = db.EmployeeInfos.Where(e => !e.Isdeleted);
        //    if (!string.IsNullOrWhiteSpace(search))
        //    {
        //        query = query.Where(e => e.Fname.Contains(search));
        //    }
        //    return query.Skip(...).Take(...).ToList(); // ❌ Query executes here!
        //}
        public IQueryable<EmployeeInfo> GetEmployeesQuery()
        {
            // Return IQueryable - NOT executed yet!
            return db.EmployeeInfos.Where(e => !e.Isdeleted);
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