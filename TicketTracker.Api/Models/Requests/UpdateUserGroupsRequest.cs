using System.ComponentModel.DataAnnotations;

namespace TicketTracker.Api.Models.Requests;

public record UpdateUserGroupsRequest(
    [Required]
    int UserId,
    [Required]
    [MinLength(1, ErrorMessage = "At least one group must be specified")]
    List<int> GroupIds
);