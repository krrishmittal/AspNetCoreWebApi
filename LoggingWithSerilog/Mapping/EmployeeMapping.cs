using AutoMapper;
using LoggingWithSerilog.DTO;
using LoggingWithSerilog.Models;

namespace LoggingWithSerilog.Mapping
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
