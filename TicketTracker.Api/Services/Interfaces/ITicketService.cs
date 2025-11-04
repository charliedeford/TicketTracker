using TicketTracker.Api.Models.Dtos;
using TicketTracker.Api.Models.Requests;
using TicketTracker.Api.Models.Responses;

public interface ITicketService
{
    Task<PaginatedResponse<TicketDto>> GetAllAsync(int page, int pageSize, int? userId, int? statusId, CancellationToken cancellationToken);
    Task<TicketDto> CreateAsync(CreateTicketRequest request, CancellationToken cancellationToken);
    Task<TicketDto> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<TicketDto> UpdateAsync(int id, UpdateTicketRequest request, CancellationToken cancellationToken);
}