using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TicketTracker.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace TicketTracker.Api.Authorization;

public class GroupAuthorizationHandler : AuthorizationHandler<GroupRequirement>
{
    private readonly TicketTrackerContext _context;

    public GroupAuthorizationHandler(TicketTrackerContext context)
    {
        _context = context;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        GroupRequirement requirement)
    {
        var user = context.User;
        if (user == null || !user.Identity!.IsAuthenticated)
        {
            return;
        }

        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return;
        }

        var userGroups = await _context.Users
            .Where(u => u.Id == userId)
            .SelectMany(u => u.Groups)
            .Select(g => g.Name)
            .ToListAsync();

        if (requirement.AllowedGroups.Any(group => userGroups.Contains(group)))
        {
            context.Succeed(requirement);
        }
    }
}

public class GroupRequirement : IAuthorizationRequirement
{
    public string[] AllowedGroups { get; }

    public GroupRequirement(string[] allowedGroups)
    {
        AllowedGroups = allowedGroups;
    }
}