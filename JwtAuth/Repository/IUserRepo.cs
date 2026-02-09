using JwtAuth.Models;

namespace JwtAuth.Repository
{
    public interface IUserRepo
    {
        IEnumerable<User> GetUser();
        User? GetUserById(int id);
        User? GetByUsername(string username);
        void AddUser(User user);
        void UpdateUser(User user);
        void DeleteUser(User user);
        void SaveChanges();
    }
}
