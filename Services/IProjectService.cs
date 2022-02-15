using BugTracker.Models;
using BugTracker.Data;

namespace BugTracker.Services;

public interface IProjectService {
    public int Create(ProjectDTO project, AppDbContext db);
    public Project GetById(int id, AppDbContext db);
    public Project GetByName(string name, AppDbContext db);
    public List<Project> GetAll(AppDbContext db);
    public Project GetByUserCreated(string name, AppDbContext db);
    public Project Update(Project newProject, AppDbContext db);
    public bool Delete(int id, AppDbContext db); 
}