using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TicketTracker.Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Policy = "RequireUser")]
public class UsersController(IUserService userService) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = "RequireSupport")]
    public async Task<ActionResult<List<UserDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var response = await userService.GetAllAsync(cancellationToken);
        return Ok(response);
    }
}
