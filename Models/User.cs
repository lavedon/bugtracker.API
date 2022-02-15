#nullable disable warnings
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace BugTracker.Models;

public class User
{
    [Key]
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }

    [JsonIgnore]
    public List<Ticket> TicketsAssigned { get; set; }
    [JsonIgnore]
    public List<Ticket> TicketsCreated { get; set; }
    [JsonIgnore]
    public List<Project> ProjectsCreated { get; set; }
}