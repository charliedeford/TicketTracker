using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TicketTracker.Api.Models.Requests;
using TicketTracker.Api.Models.Responses;

namespace TicketTracker.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("Register")]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var response = await authService.RegisterAsync(
            request.Username, 
            request.Password, 
            request.GroupIds, 
            cancellationToken);
        
        if (!response.Success) 
        {
            return BadRequest(new ErrorResponse(response.Error ?? "Registration failed"));
        }

        return Ok(new SuccessResponse("Registration successful"));
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var response = await authService.LoginAsync(
            request.Username, 
            request.Password, 
            cancellationToken);
        
        if (!response.Success) 
        {
            return Unauthorized(new ErrorResponse(response.Error ?? "Invalid credentials"));
        }

        return Ok(new AuthResponse(response.Token ?? string.Empty));
    }

    [HttpPut("Users/{userId}/Groups")]
    [Authorize(Policy = "RequireAdmin")]
    public async Task<IActionResult> UpdateUserGroups(int userId, UpdateUserGroupsRequest request, CancellationToken cancellationToken = default)
    {
        if (userId != request.UserId)
        {
            return BadRequest(new ErrorResponse("User ID mismatch"));
        }

        var response = await authService.UpdateUserGroupsAsync(userId, request.GroupIds, cancellationToken);
        
        if (!response.Success)
        {
            return BadRequest(new ErrorResponse(response.Error ?? "Failed to update user groups"));
        }

        return Ok(new SuccessResponse("User groups updated successfully"));
    }

    [HttpGet("Users/{userId}/Groups")]
    [Authorize(Policy = "RequireAdmin")]
    public async Task<IActionResult> GetUserGroups(int userId, CancellationToken cancellationToken = default)
    {
        var groups = await authService.GetUserGroupsAsync(userId, cancellationToken);
        return Ok(groups);
    }
}
