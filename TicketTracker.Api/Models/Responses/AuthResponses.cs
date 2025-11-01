namespace TicketTracker.Api.Models.Responses;

public record AuthResponse(string Token);

public record ErrorResponse(string Error);

public record SuccessResponse(string Message);