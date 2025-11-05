using Microsoft.EntityFrameworkCore;
using TicketTracker.Api.Data;

public class TicketRepository(TicketTrackerContext context) : ITicketRepository
{
    public async Task<Ticket> CreateAsync(Ticket ticket, CancellationToken cancellationToken)
    {
        context.Ticket.Add(ticket);
        await context.SaveChangesAsync(cancellationToken);

        return ticket;
    }

    public async Task<(List<Ticket>, int)> GetAllAsync(int page, int pageSize, int? userId, int? statusId, CancellationToken cancellationToken)
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

        return (tickets, totalCount);
    }

    public async Task<Ticket> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var ticket = await context.Ticket
            .Include(t => t.AssignedToUser)
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken) ?? throw new KeyNotFoundException($"Ticket with ID {id} not found.");

        return ticket;
    }

    public async Task<Ticket> UpdateAsync(Ticket ticket, CancellationToken cancellationToken)
    {
        context.Ticket.Update(ticket);
        await context.SaveChangesAsync(cancellationToken);

        return ticket;
    }
}