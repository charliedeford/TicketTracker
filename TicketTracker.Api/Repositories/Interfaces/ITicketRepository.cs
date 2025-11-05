public interface ITicketRepository
{
    Task<(List<Ticket>, int)> GetAllAsync(int page, int pageSize, int? userId, int? statusId, CancellationToken cancellationToken);
    Task<Ticket> CreateAsync(Ticket ticket, CancellationToken cancellationToken);
    Task<Ticket> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<Ticket> UpdateAsync(Ticket ticket, CancellationToken cancellationToken);
}