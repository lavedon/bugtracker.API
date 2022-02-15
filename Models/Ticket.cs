#nullable disable warnings
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace BugTracker.Models;

public class Ticket {
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int ProjectId { get; set; }
    public Project Project { get; set; }
    public int UserCreatedId { get; set; }
    public User UserCreated { get; set; }
    public int UserAssignedId { get; set; }
    public User UserAssigned { get; set; }
}