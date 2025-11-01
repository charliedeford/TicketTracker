using System.ComponentModel.DataAnnotations;

namespace TicketTracker.Api.Models.Entities;

public class Group
{
    [Key]
    public int Id { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = null!;

    [MaxLength(500)]
    public string? Description { get; set; } = null!;

    public ICollection<User> Users { get; set; } = new List<User>();
}
