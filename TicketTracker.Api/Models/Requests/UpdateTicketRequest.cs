using System.ComponentModel.DataAnnotations;

namespace TicketTracker.Api.Models.Requests;

public class UpdateTicketRequest
{
    [Required]
    public int Status { get; set; }

    public int? AssignedToUserId { get; set; }
}