using Microsoft.EntityFrameworkCore;
using TicketTracker.Api.Data;
using TicketTracker.Api.Models.Dtos;
using TicketTracker.Api.Models.Responses;

public class TicketRepository(TicketTrackerContext context) : ITicketRepository
{
    public async Task<TicketDto> CreateAsync(Ticket ticket, CancellationToken cancellationToken)
    {
        context.Ticket.Add(ticket);
        await context.SaveChangesAsync(cancellationToken);

        return new TicketDto
        {
            Id = ticket.Id,
            Title = ticket.Title,
            Description = ticket.Description,
            Status = (int)ticket.Status,
            CreatedAt = ticket.CreatedAt
        };
    }

    public async Task<PaginatedResponse<TicketDto>> GetAllAsync(int page, int pageSize, int? userId, int? statusId, CancellationToken cancellationToken)
    {
        var query = context.Ticket
            .Include(t => t.AssignedToUser)
            .AsNoTracking();

        if (userId.HasValue)
        {
            query = userId == -1 ? query.Where(t => t.AssignedToUserId == null) : query.Where(t => t.AssignedToUserId == userId.Value);
        }

        if (statusId.HasValue)
        {
            query = query.Where(t => (int)t.Status == statusId.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        
        var tickets = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var ticketResponses = tickets.Select(t => new TicketDto
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            Status = (int)t.Status,
            StatusName = t.Status.ToString(),
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt,
            AssignedToUserId = t.AssignedToUserId,
            AssignedToUsername = t.AssignedToUser?.Username
        }).ToList();

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
        var ticket = await context.Ticket
            .Include(t => t.AssignedToUser)
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken) ?? throw new KeyNotFoundException($"Ticket with ID {id} not found.");

        return new TicketDto
        {
            Id = ticket.Id,
            Title = ticket.Title,
            Description = ticket.Description,
            Status = (int)ticket.Status,
            StatusName = ticket.Status.ToString(),
            CreatedAt = ticket.CreatedAt,
            UpdatedAt = ticket.UpdatedAt,
            AssignedToUserId = ticket.AssignedToUserId,
            AssignedToUsername = ticket.AssignedToUser?.Username
        };
    }
}