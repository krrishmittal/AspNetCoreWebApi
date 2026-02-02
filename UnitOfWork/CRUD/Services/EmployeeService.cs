using AutoMapper;
using CRUD.DTO;
using CRUD.Models;
using CRUD.UnitOfWork;

namespace CRUD.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper mapper;
        public EmployeeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        public List<EmployeeRead> GetAllEmployees()
        {
            var emp = _unitOfWork.EmployeeRepository.GetEmployee();
            return mapper.Map<List<EmployeeRead>>(emp);
        }
        public EmployeeInfo AddEmployee(EmployeeCreate empDto)
        {
            var empEntity = mapper.Map<EmployeeInfo>(empDto);
            _unitOfWork.EmployeeRepository.AddEmployee(empEntity);
            _unitOfWork.Save();
            return empEntity;
        }
        public EmployeeInfo UpdateEmployee(EmployeeUpdate empDto, int id)
        {
            var emp = _unitOfWork.EmployeeRepository.GetEmployeeById(id);
            if (emp == null)
            {
                return null;
            }
            mapper.Map(empDto, emp);
            _unitOfWork.Save();
            return emp;
        }
        public bool DeleteEmployee(int id)
        {
            var emp = _unitOfWork.EmployeeRepository.GetEmployeeById(id);
            if (emp == null)
            {
                return false;
            }
            _unitOfWork.EmployeeRepository.DeleteEmployee(id);
            _unitOfWork.Save();
            return true;
        }
    }
}