using CRUD.Repositories;
using CRUD.Models;

namespace CRUD.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DumContext _context;
        private IEmployeeRepository _employeeRepository;

        public UnitOfWork(DumContext context)
        {
            _context = context;
        }

        public IEmployeeRepository EmployeeRepository
        {
            get
            {
                return _employeeRepository ??= new EmployeeRepository(_context);
            }
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
