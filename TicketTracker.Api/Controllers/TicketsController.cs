using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketTracker.Api.Data;
using TicketTracker.Api.Models.Requests;
using TicketTracker.Api.Models.Responses;

namespace TicketTracker.Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Policy = "RequireUser")]
public class TicketsController : ControllerBase
{
    private readonly TicketTrackerContext _db;

    public TicketsController(TicketTrackerContext db)
    {
        _db = db;
    }

    [HttpPost]
    public async Task<ActionResult<TicketResponse>> CreateAsync(CreateTicketRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var ticket = new Ticket
        {
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            Status = TicketStatus.Open,
            CreatedAt = DateTime.UtcNow
        };

        _db.Tickets.Add(ticket);
        await _db.SaveChangesAsync(ct);

        var response = new TicketResponse
        {
            Id = ticket.Id,
            Title = ticket.Title,
            Description = ticket.Description,
            Status = (int)ticket.Status,
            CreatedAt = ticket.CreatedAt
        };

        return CreatedAtAction(nameof(GetByIdAsync), new { id = ticket.Id }, response);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TicketResponse>> GetByIdAsync(int id, CancellationToken ct)
    {
        var ticket = await _db.Tickets.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id, ct);
        if (ticket == null) return NotFound();

        return new TicketResponse
        {
            Id = ticket.Id,
            Title = ticket.Title,
            Description = ticket.Description,
            Status = (int)ticket.Status,
            CreatedAt = ticket.CreatedAt
        };
    }
}
