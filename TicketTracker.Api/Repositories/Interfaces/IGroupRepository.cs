
using TicketTracker.Api.Models.Entities;

public interface IGroupRepository
{
    Task<Group?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Group?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<List<Group>> GetAllAsync(CancellationToken cancellationToken = default);
}