using JwtAuth.DTO;
using JwtAuth.Models;

namespace JwtAuth.Service
{
    public interface IUserService
    {
        IEnumerable<GetUser> GetUser();
        GetUser? GetUserById(int id);
        User AddUser(AddUser user);
        User UpdateUser(UpdateUser user, int id);
        bool DeleteUser(int id);
    }
}
