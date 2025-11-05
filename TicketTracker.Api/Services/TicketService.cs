using TicketTracker.Api.Models.Dtos;
using TicketTracker.Api.Models.Requests;
using TicketTracker.Api.Models.Responses;

public class TicketService(ITicketRepository ticketRepository) : ITicketService
{
    public async Task<TicketDto> CreateAsync(CreateTicketRequest request, CancellationToken cancellationToken)
    {
        var ticket = new Ticket
        {
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            Status = TicketStatus.Open,
            CreatedAt = DateTime.UtcNow
        };

        var response = await ticketRepository.CreateAsync(ticket, cancellationToken);

        return new TicketDto(response);
    }

    public async Task<PaginatedResponse<TicketDto>> GetAllAsync(int page, int pageSize, int? userId, int? statusId, CancellationToken cancellationToken)
    {
        if (page < 1)
        {
            page = 1;
        }

        if (pageSize < 1)
        {
            pageSize = 10;
        }

        if (pageSize > 100)
        {
            pageSize = 100;
        }

        var (tickets, totalCount) = await ticketRepository.GetAllAsync(page, pageSize, userId, statusId, cancellationToken);

        var ticketResponses = tickets.Select(t => new TicketDto(t)).ToList();
        
       return new PaginatedResponse<TicketDto>
        {
            Items = ticketResponses,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<TicketDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var response = await ticketRepository.GetByIdAsync(id, cancellationToken);

        return new TicketDto(response);
    }

    public async Task<TicketDto> UpdateAsync(int id, UpdateTicketRequest request, CancellationToken cancellationToken)
    {
        var ticket = await ticketRepository.GetByIdAsync(id, cancellationToken);
        if (ticket == null)
        {
            throw new KeyNotFoundException($"Ticket with id {id} not found");
        }

        ticket.Status = (TicketStatus)request.Status;
        ticket.AssignedToUserId = request.AssignedToUserId;
        ticket.UpdatedAt = DateTime.UtcNow;

        var response = await ticketRepository.UpdateAsync(ticket, cancellationToken);

        return new TicketDto(response);
    }
}