using System.Net.Http.Json;
using TicketTracker.Ui.Models;

namespace TicketTracker.Ui.Services;

public class TicketService(HttpClient httpClient) : ITicketService
{
    public async Task<bool> CreateAsync(TicketCreateRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync("tickets", request, cancellationToken);

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[TicketService] CreateAsync exception: {ex.Message}");
            return false;
        }
    }

    public async Task<PaginatedResponse<TicketDto>?> GetAllAsync(int page = 1, int pageSize = 10, int? userId = null, int? statusId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.GetFromJsonAsync<PaginatedResponse<TicketDto>>(
                $"tickets?page={page}&pageSize={pageSize}&userId={userId}&statusId={statusId}", cancellationToken);
            return response;
        }
        catch
        {
            return null;
        }
    }

    public async Task<TicketDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.GetFromJsonAsync<TicketDto>($"tickets/{id}", cancellationToken);
            return response;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> UpdateAsync(int id, UpdateTicketRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.PutAsJsonAsync($"tickets/{id}", request, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[TicketService] UpdateAsync exception: {ex.Message}");
            return false;
        }
    }
}
