namespace TicketTracker.Api.Models.Dtos;

public class TicketDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? AssignedToUserId { get; set; }
    public string? AssignedToUsername { get; set; }

    public TicketDto(Ticket ticket)
    {
        Id = ticket.Id;
        Title = ticket.Title;
        Description = ticket.Description;
        Status = (int)ticket.Status;
        StatusName = ticket.Status.ToString();
        CreatedAt = ticket.CreatedAt;
        UpdatedAt = ticket.UpdatedAt;
        AssignedToUserId = ticket.AssignedToUserId;
        AssignedToUsername = ticket.AssignedToUser?.Username;
    }
}
