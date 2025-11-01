namespace TicketTracker.Ui.Models;

public record RegisterRequest(
    string Username,
    string Password,
    int[] GroupIds
);