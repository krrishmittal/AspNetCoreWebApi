using AutoMapper;
using ValidationForDataModels.DTO;
using ValidationForDataModels.Models;

namespace LoggingWithSerilog.Mapping
{
    public class EmployeeMapping : Profile
    {
        public EmployeeMapping()
        {
            CreateMap<User, GetUser>();
            CreateMap<AddUser, User>();
            CreateMap<UpdateUser, User>();
        }
    }
}