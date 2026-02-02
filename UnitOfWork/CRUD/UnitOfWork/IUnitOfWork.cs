using CRUD.Repositories;
namespace CRUD.UnitOfWork
{
    public interface IUnitOfWork
    {
        IEmployeeRepository EmployeeRepository { get; }
        void Save();
    }
}
