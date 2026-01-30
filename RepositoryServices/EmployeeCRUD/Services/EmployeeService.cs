using AutoMapper;
using EmployeeCRUD.DTO;
using EmployeeCRUD.Models;
using EmployeeCRUD.Repositories;

namespace EmployeeCRUD.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository repository;
        private readonly IMapper mapper;
        public EmployeeService(IEmployeeRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }
        public List<EmployeeRead> GetAllEmployees()
        {
            var emp = repository.GetEmployee();
            return mapper.Map<List<EmployeeRead>>(emp);
        }
        public EmployeeInfo AddEmployee(EmployeeCreate empDto)
        {
            var empEntity = mapper.Map<EmployeeInfo>(empDto);
            repository.AddEmployee(empEntity);
            repository.Save();
            return empEntity;
        }
        public EmployeeInfo UpdateEmployee(EmployeeUpdate empDto, int id)
        {
            var emp = repository.GetEmployeeById(id);
            if (emp == null)
            {
                return null;
            }
            mapper.Map(empDto, emp);
            repository.Save();
            return emp;
        }
        public bool DeleteEmployee(int id)
        {
            var emp = repository.GetEmployeeById(id);
            if (emp == null)
            {
                return false;
            }
            repository.DeleteEmployee(id);
            repository.Save();
            return true;
        }
    }
}