using System.ComponentModel.DataAnnotations;

namespace TicketTracker.Api.Models.Requests;

public record LoginRequest(
    [Required(ErrorMessage = "Username is required")]
    string Username,
    [Required(ErrorMessage = "Password is required")]
    string Password);