#nullable disable warnings
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BugTracker.Models;

public class Project {
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int UserCreatedId { get; set; }
    public User UserCreated { get; set; }

    [JsonIgnore]
    public List<Ticket> Tickets { get; set; }
}