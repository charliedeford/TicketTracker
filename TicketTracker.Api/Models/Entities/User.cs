using System.ComponentModel.DataAnnotations;

namespace TicketTracker.Api.Models.Entities;

public class User
{
    [Key]
    public int Id { get; set; }
    
    [MaxLength(450)]
    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public ICollection<Group> Groups { get; set; } = [];
    public ICollection<Ticket>? Tickets { get; set; } = [];
}