namespace TicketTracker.Ui.Models;

public class UpdateTicketRequest
{
    public int Status { get; set; }
    public int? AssignedToUserId { get; set; }
}