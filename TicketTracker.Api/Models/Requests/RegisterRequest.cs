using System.ComponentModel.DataAnnotations;

namespace TicketTracker.Api.Models.Requests;

public record RegisterRequest(
    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Username must be between 1 and 50 characters")]
    string Username,
    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Password must be between 1 and 100 characters")]
    string Password,
    int[]? GroupIds);