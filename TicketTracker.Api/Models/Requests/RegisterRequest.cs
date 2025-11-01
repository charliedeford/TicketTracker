namespace TicketTracker.Api.Models.Requests;

public record RegisterRequest(string Username, string Password, int[]? GroupIds);