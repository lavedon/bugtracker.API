#nullable disable warnings
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BugTracker.Models;

public class Project {
    [Key]
    public int? Id { get; set; } = null;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? UserCreatedId { get; set; } = null;
    [ForeignKey("UserCreatedId")]
    public User UserCreated { get; set; }

    [JsonIgnore]
    public List<Ticket> Tickets { get; set; }
}