using CRM.Core.Enums;
using CRM.Core.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace CRM.API.Configuration.Filters;

[AttributeUsage(AttributeTargets.Method)]
public class AuthorizationFilterByLeadId : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var currentUserId = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var requestId = context.HttpContext.Request.RouteValues[RequestRouteKeys.Id] as string;
        if (!context.HttpContext.User.IsInRole(nameof(LeadStatus.Administrator))
            && currentUserId != requestId)
        {
            throw new UnauthorizedException();
        }
    }
}
