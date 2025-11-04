using TicketTracker.Api.Models.Dtos;
using TicketTracker.Api.Models.Responses;

public interface ITicketRepository
{
    Task<PaginatedResponse<TicketDto>> GetAllAsync(int page, int pageSize, int? userId, int? statusId, CancellationToken cancellationToken);
    Task<TicketDto> CreateAsync(Ticket ticket, CancellationToken cancellationToken);
    Task<TicketDto> GetByIdAsync(int id, CancellationToken cancellationToken);
}