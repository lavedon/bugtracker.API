using BugTracker.Models;
using BugTracker.Data;

namespace BugTracker.Services;

public interface ITicketService {
    public Ticket Create(Ticket ticket, AppDbContext db);
    public Ticket Get(int id, AppDbContext db);
    public Ticket Get(string name, AppDbContext db);
    public List<Ticket> GetAll(AppDbContext db);
    public List<Ticket> GetByUserCreated(string name, AppDbContext db);
    public List<User> GetAssignedUsers (string name, AppDbContext db);
    public List<Ticket> GetByProject(string name, AppDbContext db);
    public bool Update(Ticket ticket, AppDbContext db);
    public bool Delete(string name, AppDbContext db); 
}