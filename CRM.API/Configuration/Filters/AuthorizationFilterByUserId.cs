using CRM.Core.Constants;
using CRM.Core.Exсeptions;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace CRM.API.Configuration.Filters;

[AttributeUsage(AttributeTargets.Method)]
public class AuthorizationFilterByUserId : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var currentUserId = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var requestId = context.HttpContext.Request.Path.Value.ToString()[$"{ControllersRoutes.LeadsController}/".Length..].Trim();
        if (currentUserId != requestId)
        {
            throw new UnauthorizedException();
        }
    }
}
