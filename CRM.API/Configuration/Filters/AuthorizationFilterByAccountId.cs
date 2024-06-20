using System.Security.Claims;
using System.Text.Json;
using CRM.Business.Interfaces;
using CRM.Business.Models.Accounts.Responses;
using CRM.Core;
using CRM.Core.Enums;
using CRM.Core.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRM.API.Configuration.Filters;

[AttributeUsage(AttributeTargets.Method)]
public class AuthorizationFilterByAccountId : Attribute, IAsyncAuthorizationFilter
{
    private readonly JsonSerializerOptions _options = JsonSerializerOptionsProvider.GetJsonSerializerOptions();
    
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var currentUserId = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (context.HttpContext.Request.RouteValues[RequestRouteKeys.Id] is string requestId)
        {
            var account = await GetAccount(new Guid(requestId), context.HttpContext);
            if (!context.HttpContext.User.IsInRole(nameof(LeadStatus.Administrator))
                && currentUserId != account.LeadId.ToString())
            {
                throw new UnauthorizedException();
            }
        }
    }

    private static async Task<AccountForAuthorizationFilterResponse> GetAccount(Guid accountId, HttpContext context)
    {
        var accountsService = context.RequestServices.GetRequiredService<IAccountsService>();
        var account = await accountsService.GetAccountByIdAsync<AccountForAuthorizationFilterResponse>(accountId);

        return account;
    }
}
