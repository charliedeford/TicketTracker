using TicketTracker.Api.Models.Entities;
using TicketTracker.Api.Models.Responses;

public interface IAuthService
{
    Task<RegisterResponse> RegisterAsync(string username, string password, int[]? groupIds, CancellationToken cancellationToken = default);
    Task<LoginResponse> LoginAsync(string username, string password, CancellationToken cancellationToken = default);
    Task<LoginResponse> UpdateUserGroupsAsync(int userId, List<int> groupIds, CancellationToken cancellationToken = default);
    Task<List<Group>> GetUserGroupsAsync(int userId, CancellationToken cancellationToken = default);
}