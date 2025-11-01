namespace TicketTracker.Api.Models.Responses;

public class RegisterResponse(bool success, string? error)
{
    public bool Success { get; set; } = success;
    public string? Error { get; set; } = error;
}