#nullable disable warnings
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace BugTracker.Models;

public class Ticket {
    [Key]
    public int? Id { get; set; } = null;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? ProjectId { get; set; } = null;
    public Project Project { get; set; }
    public int? UserCreatedId { get; set; } = null;
    [ForeignKey("UserCreatedId")]
    public User UserCreated { get; set; }
    public int? UserAssignedId { get; set; } = null;
    [ForeignKey("UserAssignedId")]
    public User UserAssigned { get; set; }
}