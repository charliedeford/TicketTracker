using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketTracker.Api.Models.Dtos;
using TicketTracker.Api.Models.Requests;
using TicketTracker.Api.Models.Responses;

namespace TicketTracker.Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Policy = "RequireUser")]
public class TicketsController(ITicketService ticketService) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = "RequireSupport")]
    public async Task<ActionResult<PaginatedResponse<TicketDto>>> GetAllAsync(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? userId = null,
        [FromQuery] int? statusId = null,
        CancellationToken cancellationToken = default)
    {
        var response = await ticketService.GetAllAsync(page, pageSize, userId, statusId, cancellationToken);
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<TicketDto>> CreateAsync(CreateTicketRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var response = await ticketService.CreateAsync(request, cancellationToken);       

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TicketDto>> GetByIdAsync(int id, CancellationToken ct)
    {
        var response = await ticketService.GetByIdAsync(id, ct);
        return Ok(response);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TicketDto>> UpdateAsync(int id, UpdateTicketRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var response = await ticketService.UpdateAsync(id, request, ct);
        return Ok(response);
    }
}
