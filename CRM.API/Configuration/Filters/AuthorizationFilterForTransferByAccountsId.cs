using System.Security.Claims;
using System.Text.Json;
using CRM.Business.Interfaces;
using CRM.Business.Models.Accounts.Responses;
using CRM.Business.Models.Transactions.Requests;
using CRM.Core;
using CRM.Core.Enums;
using CRM.Core.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRM.API.Configuration.Filters;

[AttributeUsage(AttributeTargets.Method)]
public class AuthorizationFilterForTransferByAccountsId : Attribute, IAsyncAuthorizationFilter
{
    private readonly JsonSerializerOptions _options = JsonSerializerOptionsProvider.GetJsonSerializerOptions();
    
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var currentUserId = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var requestBody = await StreamReaderFromRequest.StreamReaderRequestBody(context.HttpContext.Request);
        var (accountToId, accountFromId) = await GetAccount(requestBody, context.HttpContext);
        if (!context.HttpContext.User.IsInRole(nameof(LeadStatus.Administrator))
            && currentUserId != accountToId.LeadId.ToString()
            && currentUserId != accountFromId.LeadId.ToString())
        {
            throw new UnauthorizedException();
        }
    }

    private async Task<(AccountForAuthorizationFilterResponse, AccountForAuthorizationFilterResponse)> GetAccount(string requestBody, HttpContext context)
    {
        var request = JsonSerializer.Deserialize<TransferRequest>(requestBody, _options);
        var accountsService = context.RequestServices.GetRequiredService<IAccountsService>();
        var accountToId = await accountsService.GetAccountByIdAsync<AccountForAuthorizationFilterResponse>(request.AccountToId);
        var accountFromId = await accountsService.GetAccountByIdAsync<AccountForAuthorizationFilterResponse>(request.AccountFromId);

        return (accountToId, accountFromId);
    }
}
