using ValidationForDataModels.Models;
using ValidationForDataModels.Repository;
namespace ValidationForDataModels.Repository
{
    public class UserRepo:IUserRepo
    {
        private readonly AppdbContext db;
        public UserRepo(AppdbContext db) 
        {
            this.db = db;
        }
        public IEnumerable<User> GetUser()
        {
            return db.Users;
        }
        public User? GetUserById(int id)
        {
            return db.Users.Find(id);
        }
        public void AddUser(User user)
        {
            db.Users.Add(user);
        }

        public void UpdateUser(User user)
        {
            db.Users.Update(user);
        }
        public void DeleteUser(User user)
        {
            var existingUser = db.Users.Find(user.Id);
            if (existingUser != null)
            {
                existingUser.Isdeleted = true;
                db.Users.Update(existingUser);
            }
        }
        public void SaveChanges()
        {
            db.SaveChanges();
        }
    }
}
