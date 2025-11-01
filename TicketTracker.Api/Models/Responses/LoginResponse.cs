namespace TicketTracker.Api.Models.Responses;

public class LoginResponse(bool success, string? token, string? error)
{
    public bool Success { get; set; } = success;
    public string? Token { get; set; } = token;
    public string? Error { get; set; } = error;
}