using TicketTracker.Ui.Models;

namespace TicketTracker.Ui.Services;

public interface IAuthService
{
    Task<bool> LoginAsync(LoginRequest request);
    Task<bool> RegisterAsync(RegisterRequest request);
    Task LogoutAsync();
    bool IsAuthenticated { get; }
}