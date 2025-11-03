using TicketTracker.Ui.Models;

namespace TicketTracker.Ui.Services;

public interface ITicketService
{
    Task<bool> CreateAsync(TicketCreateRequest request, CancellationToken ct = default);
}
