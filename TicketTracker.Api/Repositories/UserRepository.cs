using Microsoft.EntityFrameworkCore;
using TicketTracker.Api.Data;
using TicketTracker.Api.Models.Entities;

namespace TicketTracker.Api.Repositories;

public class UserRepository(TicketTrackerContext context) : IUserRepository
{
    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await context.Users
            .Include(u => u.Groups)
            .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
    }

    public async Task<User?> GetByIdWithGroupsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await context.Users
            .Include(u => u.Groups)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default)
    {
        return await context.Users.AnyAsync(u => u.Username == username, cancellationToken);
    }

    public async Task<User> CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        context.Users.Add(user);
        await SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        context.Entry(user).State = EntityState.Modified;
        await SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await context.Users
            .Include(u => u.Groups)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task UpdateUserGroupsAsync(int userId, List<int> groupIds, CancellationToken cancellationToken = default)
    {
        var user = await context.Users
            .Include(u => u.Groups)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found");
        }

        var groups = await context.Groups
            .Where(g => groupIds.Contains(g.Id))
            .ToListAsync(cancellationToken);

        user.Groups.Clear();
        foreach (var group in groups)
        {
            user.Groups.Add(group);
        }

        await SaveChangesAsync(cancellationToken);
    }

    public async Task<List<UserDto>?> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Users
            .Select(u => new UserDto(u.Id, u.Username))
            .ToListAsync(cancellationToken);
    }
}