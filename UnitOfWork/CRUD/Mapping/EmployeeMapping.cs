using AutoMapper;
using CRUD.DTO;
using CRUD.Models;

namespace CRUD.Mapping
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
