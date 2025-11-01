namespace TicketTracker.Api.Models.Requests;

public record UpdateUserGroupsRequest(
    int UserId,
    List<int> GroupIds
);