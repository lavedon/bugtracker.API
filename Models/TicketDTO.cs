#nullable disable warnings
using System.ComponentModel.DataAnnotations;
namespace BugTracker.Models;

public class TicketDTO {
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Project { get; set; }
    public string? UserCreated { get; set; }
    public string? UserAssigned { get; set; }
}