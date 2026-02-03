using AutoMapper;
using CRUD.DTO;
using CRUD.Models;
using CRUD.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CRUD.Services
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

        // OLD Service - just returned list
        //public List<EmployeeRead> GetEmployeesPaged(int pageNumber, int pageSize, string search)
        //{
        //    var items = repository.GetEmployeePaged(pageNumber, pageSize, search);
        //    return mapper.Map<List<EmployeeRead>>(items);
        //}
        public async Task<PaginatedResult<EmployeeRead>> GetAllEmployeesAsync(
            string? filterOn = null,
            string? filterQuery = null,
            string? sortBy = null,
            bool isAscending = true,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var employeesQuery = repository.GetEmployeesQuery();

            
            if (!string.IsNullOrWhiteSpace(filterOn) && !string.IsNullOrWhiteSpace(filterQuery))
            {
                if (filterOn.Equals("Fname", StringComparison.OrdinalIgnoreCase))
                {
                    employeesQuery = employeesQuery
                        .Where(emp => emp.Fname.ToLower().Contains(filterQuery.ToLower()));
                }

                if (filterOn.Equals("Lname", StringComparison.OrdinalIgnoreCase))
                {
                    employeesQuery = employeesQuery
                        .Where(emp => emp.Lname.ToLower().Contains(filterQuery.ToLower()));
                }
            }

            
            var totalCount = await employeesQuery.CountAsync();

            
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                if (sortBy.Equals("Fname", StringComparison.OrdinalIgnoreCase))
                {
                    employeesQuery = isAscending
                        ? employeesQuery.OrderBy(emp => emp.Fname)
                        : employeesQuery.OrderByDescending(emp => emp.Fname);
                }
                else if (sortBy.Equals("Lname", StringComparison.OrdinalIgnoreCase))
                {
                    employeesQuery = isAscending
                        ? employeesQuery.OrderBy(emp => emp.Lname)
                        : employeesQuery.OrderByDescending(emp => emp.Lname);
                }
                else if (sortBy.Equals("Id", StringComparison.OrdinalIgnoreCase))
                {
                    employeesQuery = isAscending
                        ? employeesQuery.OrderBy(emp => emp.Id)
                        : employeesQuery.OrderByDescending(emp => emp.Id);
                }
            }
            else
            {
                
                employeesQuery = employeesQuery.OrderBy(emp => emp.Id);
            }
             
            var skipResults = (pageNumber - 1) * pageSize;
            var employees = await employeesQuery
                .Skip(skipResults)
                .Take(pageSize)
                .ToListAsync();  
            var employeeDtos = mapper.Map<List<EmployeeRead>>(employees);

            
            return new PaginatedResult<EmployeeRead>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                Data = employeeDtos
            };
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