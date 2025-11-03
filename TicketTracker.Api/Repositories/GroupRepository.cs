using Microsoft.EntityFrameworkCore;
using TicketTracker.Api.Data;
using TicketTracker.Api.Models.Entities;

public class GroupRepository(TicketTrackerContext context) : IGroupRepository
{
    public async Task<Group?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await context.Groups
            .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
    }

    public async Task<Group?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await context.Groups
            .FirstOrDefaultAsync(g => g.Name == name, cancellationToken);
    }

    public async Task<List<Group>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Groups
            .ToListAsync(cancellationToken);
    }
}