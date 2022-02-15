using BugTracker.Models;
using BugTracker.Data;

namespace BugTracker.Services;

public interface IUserService
{
    public User Get(UserLogin userLogin, AppDbContext db);

    public List<User> GetAll(AppDbContext db);

    public User GetByName(string name, AppDbContext db);



}