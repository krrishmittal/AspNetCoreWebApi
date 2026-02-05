using ValidationForDataModels.Models;
using ValidationForDataModels.DTO;

namespace ValidationForDataModels.Service
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
