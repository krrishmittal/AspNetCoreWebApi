using GoogleLogin.Models;

namespace GoogleLogin.Repository
{
    public interface IUserRepo
    {
        IEnumerable<User> GetUser();
        User? GetUserById(int id);
        User? GetByUsername(string username);   
        User? GetByEmail(string email);
        void AddUser(User user);
        void UpdateUser(User user);
        void DeleteUser(User user);
        void SaveChanges();
    }
}
