#nullable disable warnings
using BugTracker.Models;
using BugTracker.Data;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Services;

public class TicketService : ITicketService
{
    public Ticket Create(Ticket ticket, AppDbContext db)
    {
        db.Tickets.Add(ticket);
        db.SaveChanges();
        return ticket;
    }

    public bool Delete(string name, AppDbContext db)
    {
        Ticket ticketToDelete = db.Tickets.FirstOrDefault(t => t.Name.Equals(name));
        if (ticketToDelete == null)
        {
            return false;
        }
        db.Tickets.Remove(ticketToDelete);
        db.SaveChanges();
        return true;
    }
    public bool Delete(int id, AppDbContext db)
    {
        
        var ticketToDelete = db.Tickets.Include(t => t.Project)
            .Include(t => t.UserCreated)
            .Include(t => t.UserAssigned)
            .Single(t => t.Id == id);
        if (ticketToDelete is DBNull)
        {
            return false;
        }
        db.Tickets.Remove(ticketToDelete);
        db.SaveChanges();
        return true;
    }

    public Ticket Get(int id, AppDbContext db)
    {
        Ticket result = db.Tickets.Find(id);
        return result;
    }

    public Ticket Get(string name, AppDbContext db)
    {
        Ticket result = db.Find<Ticket>(name);
        return result;
    }

    public List<Ticket> GetAll(AppDbContext db)
    {
        List<Ticket> tickets =  db.Tickets
            .AsNoTracking()
            .Include(ticket => ticket.Project)
            .Include(ticket => ticket.UserCreated)
            .Include(ticket => ticket.UserAssigned)
            .ToList();

        foreach (var ticket in tickets)
        {
            ticket.UserCreated.Password = "";
            ticket.UserAssigned.Password = "";
        }

        return tickets;
    }

    public List<User> GetAssignedUsers(string name, AppDbContext db)
    {
        throw new NotImplementedException();
    }

    public List<Ticket> GetByProject(string name, AppDbContext db)
    {
        int idOfProject = db.Projects.FirstOrDefault(p => p.Name.Equals(name)).Id;
        return db.Tickets.Where(t => t.ProjectId == idOfProject).ToList();
    }

    public List<Ticket> GetByUserCreated(string name, AppDbContext db)
    {
        return db.Tickets.Where(t => t.UserCreated.Equals(name)).ToList();
    }

    public bool Update(Ticket ticket, AppDbContext db)
    {
         db.Tickets.Update(ticket);
         return true;
    }
}