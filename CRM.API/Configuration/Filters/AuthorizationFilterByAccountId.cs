using System.Security.Claims;
using CRM.Business.Interfaces;
using CRM.Business.Models.Accounts.Responses;
using CRM.Core.Enums;
using CRM.Core.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRM.API.Configuration.Filters;

[AttributeUsage(AttributeTargets.Method)]
public class AuthorizationFilterByAccountId : Attribute, IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var currentUserId = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var requestId = context.HttpContext.Request.RouteValues[RequestRouteKeys.Id] as string;
        var accountsService = context.HttpContext.RequestServices.GetRequiredService<IAccountsService>();
        if (string.IsNullOrEmpty(requestId))
        {
            throw new UnauthorizedException();
        }

        var account = await accountsService.GetAccountByIdAsync<AccountForAuthorizationFilterResponse>(new Guid(requestId));
        if (!context.HttpContext.User.IsInRole(nameof(LeadStatus.Administrator))
            && currentUserId != account.LeadId.ToString())
        {
            throw new UnauthorizedException();
        }
    }
}
