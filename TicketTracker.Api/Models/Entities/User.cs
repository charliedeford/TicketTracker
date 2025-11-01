namespace TicketTracker.Api.Models.Entities;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public ICollection<Group> Groups { get; set; } = new List<Group>();
}
