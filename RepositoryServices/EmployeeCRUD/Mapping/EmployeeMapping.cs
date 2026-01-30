using EmployeeCRUD.Models;
using EmployeeCRUD.DTO;
using AutoMapper;
namespace EmployeeCRUD.Mapping
{
    public class EmployeeMapping : Profile
    {
        public EmployeeMapping()
        {
            CreateMap<EmployeeInfo, EmployeeRead>();
            CreateMap<EmployeeCreate, EmployeeInfo>();
            CreateMap<EmployeeUpdate, EmployeeInfo>();
        }
    }
}
