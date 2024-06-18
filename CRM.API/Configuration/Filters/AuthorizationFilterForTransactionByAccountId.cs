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
public class AuthorizationFilterForTransactionByAccountId : Attribute, IAsyncAuthorizationFilter
{
    private readonly JsonSerializerOptions _options = JsonSerializerOptionsProvider.GetJsonSerializerOptions();
    
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var currentUserId = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var requestBody = await StreamReaderFromRequest.StreamReaderRequestBody(context.HttpContext.Request);
        var account = await GetAccount(requestBody, context.HttpContext);
        if (!context.HttpContext.User.IsInRole(nameof(LeadStatus.Administrator))
            && currentUserId != account.LeadId.ToString())
        {
            throw new UnauthorizedException();
        }
    }

    private async Task<AccountForAuthorizationFilterResponse> GetAccount(string requestBody, HttpContext context)
    {
        var accountId = JsonSerializer.Deserialize<TransactionRequest>(requestBody, _options).AccountId;
        var accountsService = context.RequestServices.GetRequiredService<IAccountsService>();
        var account = await accountsService.GetAccountByIdAsync<AccountForAuthorizationFilterResponse>(accountId);

        return account;
    }
}
