using System.Net.Http.Json;
using TicketTracker.Ui.Models;

namespace TicketTracker.Ui.Services;

public class TicketService : ITicketService
{
    private readonly HttpClient _httpClient;

    public TicketService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> CreateAsync(TicketCreateRequest request, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("tickets", request, ct);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
