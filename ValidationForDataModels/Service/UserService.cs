using ValidationForDataModels.Models;
using ValidationForDataModels.Repository;
using ValidationForDataModels.DTO;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ValidationForDataModels.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepo userRepo;
        private readonly IMapper mapper;
        
        public UserService(IUserRepo userRepo, IMapper mapper)
        {
            this.userRepo = userRepo;
            this.mapper = mapper;
        }

        public IEnumerable<GetUser> GetUser()
        {
            var users = userRepo.GetUser().Where(u => !u.Isdeleted);
            return mapper.Map<IEnumerable<GetUser>>(users);
        }

        public GetUser? GetUserById(int id)
        {
            var user = userRepo.GetUserById(id);
            if (user == null || user.Isdeleted)
            {
                return null;
            }
            return mapper.Map<GetUser>(user);
        }

        public User AddUser(AddUser userDto)
        {
            var user = mapper.Map<User>(userDto);
            userRepo.AddUser(user);
            userRepo.SaveChanges();
            return user;
        }

        public User UpdateUser(UpdateUser userDto, int id)
        {
            var existingUser = userRepo.GetUserById(id);
            if (existingUser == null || existingUser.Isdeleted)
            {
                throw new Exception("User not found");
            }

            // ✅ Only update fields that are provided
            if (!string.IsNullOrEmpty(userDto.Username))
            {
                existingUser.Username = userDto.Username;
            }

            if (!string.IsNullOrEmpty(userDto.Password))
            {
                existingUser.Password = userDto.Password;
            }

            userRepo.UpdateUser(existingUser);
            userRepo.SaveChanges();
            return existingUser;
        }

        public bool DeleteUser(int id)
        {
            var existingUser = userRepo.GetUserById(id);
            if (existingUser == null || existingUser.Isdeleted)
            {
                return false;
            }
            existingUser.Isdeleted = true;
            userRepo.SaveChanges();
            return true;
        }
    }
}
