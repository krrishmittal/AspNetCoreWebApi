using ValidationForDataModels.Models;
namespace ValidationForDataModels.Repository
{
    public interface IUserRepo
    {
        IEnumerable<User> GetUser();
        User? GetUserById(int id);
        void AddUser(User user);
        void UpdateUser(User user);
        void DeleteUser(User user);
        void SaveChanges();
    }
}
