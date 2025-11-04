using System.Net.Http.Json;
using TicketTracker.Ui.Services;

public class UserService(HttpClient httpClient) : IUserService
{
    public async Task<List<UserDto>?> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync("users", cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadFromJsonAsync<List<UserDto>>(cancellationToken: cancellationToken);
            return content;
        }
        return null;
    }
}