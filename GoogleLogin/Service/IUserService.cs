using GoogleLogin.DTO;
using GoogleLogin.Models;

namespace GoogleLogin.Service
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
