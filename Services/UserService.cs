#nullable disable warnings
using BugTracker.Models;
using BugTracker.Data;

namespace BugTracker.Services;

public class UserService : IUserService
{
    public User Get(UserLogin userLogin, AppDbContext db)
    {
        User user = db.Users.FirstOrDefault(o => o.Username.Equals
            (userLogin.Username) && o.Password.Equals
            (userLogin.Password));

        return user;
    }

    public List<User> GetAll(AppDbContext db)
    {
        List<User> allUsers = db.Users.ToList();
        foreach(var user in allUsers)
        {
            user.Password = "";
        }

        return allUsers;
    }

    public User GetByName(string name, AppDbContext db)
    {
        return db.Users.FirstOrDefault(o => o.Username.Equals(name)); 
    }
}