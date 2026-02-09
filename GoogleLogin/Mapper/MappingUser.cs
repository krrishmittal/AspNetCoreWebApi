using AutoMapper;
using GoogleLogin.DTO;
using GoogleLogin.Models;

namespace GoogleLogin.Mapping
{
    public class MappingUser : Profile
    {
        public MappingUser()
        {
            CreateMap<User, GetUser>();
            CreateMap<AddUser, User>();
            CreateMap<UpdateUser, User>();
        }
    }
}