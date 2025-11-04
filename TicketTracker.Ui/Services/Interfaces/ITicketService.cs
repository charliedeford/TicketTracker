using TicketTracker.Ui.Models;

namespace TicketTracker.Ui.Services;

public interface ITicketService
{
    Task<bool> CreateAsync(TicketCreateRequest request, CancellationToken cancellationToken = default);
    Task<PaginatedResponse<TicketDto>?> GetAllAsync(int page = 1, int pageSize = 10, int? userId = null, int? statusId = null, CancellationToken cancellationToken = default);
    Task<TicketDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, UpdateTicketRequest request, CancellationToken cancellationToken = default);
}
