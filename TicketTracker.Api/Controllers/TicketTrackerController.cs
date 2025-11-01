using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace TicketTracker.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class TicketTrackerController(IAuthService authService) : ControllerBase
{
}
