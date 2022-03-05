#nullable disable warnings
using BugTracker.Models;
using BugTracker.Data;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Services;

public class ProjectService : IProjectService
{
    public int? Create(ProjectDTO projectDTO, AppDbContext db)
    {
        User userCreated = db.Users.FirstOrDefault(u => u.Username.Equals(projectDTO.UserCreated));


        Project _project = new Project {
            Name = projectDTO.Name,
            Description = projectDTO.Description,
            UserCreated = db.Users.Include(u => u.ProjectsCreated).FirstOrDefault(u => u.Username.Equals(projectDTO.UserCreated)),
            UserCreatedId = userCreated.Id
        };
        db.Projects.Add(_project);
        db.SaveChanges();
        return _project.Id;
    }

    public bool Delete(int id, AppDbContext db)
    {

        var project = db.Projects.Include(p => p.Tickets)
             .Single(p => p.Id == id);

       if (project is null) return false;
        
        try { 
            db.Projects.Remove(project);
            db.SaveChanges();
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Tried to delete project: {id}" + ex);
        }
       return true;
    }

    public Project GetById(int id, AppDbContext db)
    {
        Project project = db.Projects.FirstOrDefault(o => o.Id == id);
        return project!;
    }

    public Project GetByName(string name, AppDbContext db)
    {
        Project project = db.Projects.FirstOrDefault(o => o.Name.Equals(name));
        return project!;
    }

    public List<Project> GetAll(AppDbContext db)
    {
        var projects = db.Projects.Include(p => p.UserCreated).ToList();
        foreach (var project in projects)
        {
            project.UserCreated.Password = "";
        }

        return projects!;
    }

    public Project GetByUserCreated(string name, AppDbContext db)
    {
        Project project = db.Projects.FirstOrDefault(o => o.UserCreated.Username.Equals(name));
        return project!;
    }

    public Project Update(Project newProject, AppDbContext db)
    {
        Project oldProject = db.Projects.FirstOrDefault(p => p.Id == newProject.Id);

        if (oldProject is null) return null;

        oldProject.Name = newProject.Name;
        oldProject.Description = newProject.Description;
        db.SaveChanges();
        // oldProject.UserCreated = db.Users.FirstOrDefault(u => u.Username.Equals(newProject.UserCreated));

        return oldProject;
    }
}