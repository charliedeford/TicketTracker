using System.ComponentModel.DataAnnotations;
using TicketTracker.Api.Models.Entities;

public class Ticket
{
    [Key]
    public int Id { get; set; }

    [MaxLength(200)]
    public string Title { get; set; } = null!;

    [MaxLength(2000)]
    public string Description { get; set; } = null!;
    public TicketStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public int? AssignedToUserId { get; set; }
    public User? AssignedToUser { get; set; } = null!;
}

public enum TicketStatus
{
    [Display(Name = "Open")]
    Open = 0,

    [Display(Name = "In Progress")]
    InProgress = 1,

    [Display(Name = "Closed")]
    Closed = 2
}