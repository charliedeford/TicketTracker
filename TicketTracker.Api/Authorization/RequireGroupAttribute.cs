using Microsoft.AspNetCore.Authorization;

namespace TicketTracker.Api.Authorization;

public class RequireGroupAttribute : AuthorizeAttribute
{
    public RequireGroupAttribute(params string[] groups) 
        : base(policy: string.Join(",", groups))
    {
    }
}