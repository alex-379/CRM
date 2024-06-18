using CRM.API.Configuration.Filters;
using CRM.API.Controllers.Constants;
using CRM.API.Controllers.Constants.Logs;
using CRM.Business.Configuration.HttpClients;
using CRM.Business.Interfaces;
using CRM.Business.Models.Accounts.Requests;
using CRM.Business.Models.Transactions.Responses;
using CRM.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace CRM.API.Controllers;

[Authorize]
[ApiController]
[Route(Routes.AccountsController)]
public class AccountsController(IAccountsService accountsService, IHttpClientService<TransactionStoreHttpClient> httpClientService) : Controller
{
    private readonly Serilog.ILogger _logger = Log.ForContext<AccountsController>();

    [HttpPost]
    public async Task<ActionResult<Guid>> RegisterAccountAsync([FromBody] RegisterAccountRequest request)
    {
        _logger.Information(LeadsLogs.GetAuthorizedLead);
        var currentLeadId = InformationCurrentLead.GetCurrentLeadFromClaims(HttpContext.User); 
        _logger.Information(AccountsLogs.RegisterAccount, request.Currency, currentLeadId);
        var id = await accountsService.AddAccountAsync(currentLeadId, request);

        return Created($"{Routes.Host}{Routes.LeadsController}/{id}", id);
    }
    
    [Authorize(Roles = nameof(LeadStatus.Administrator))]
    [HttpPatch(Routes.Status)]
    public async Task<ActionResult> UpdateAccountStatusAsync([FromRoute] Guid id, [FromBody] UpdateAccountStatusRequest request)
    {
        _logger.Information(AccountsLogs.UpdateAccountStatus, id);
        await accountsService.UpdateAccountStatusAsync(id, request);

        return NoContent();
    }
    
    [AuthorizationFilterByAccountId]
    [HttpGet(Routes.Transactions)]
    public async Task<ActionResult<List<TransactionResponse>>> GetTransactionsByAccountId(Guid id)
    {
        _logger.Information(AccountsLogs.GetTransactions, id);
        var transactions = await httpClientService.GetAsync<List<TransactionResponse>>(string.Format(Routes.TransactionsTStore, id));

        return Ok(transactions);
    }
    
    [AuthorizationFilterByAccountId]
    [HttpGet(Routes.Balance)]
    public async Task<ActionResult<AccountBalanceResponse>> GetBalanceByAccountId(Guid id)
    {
        _logger.Information(AccountsLogs.GetBalance, id);
        var balance = await httpClientService.GetAsync<AccountBalanceResponse>(string.Format(Routes.BalanceTStore, id));

        return Ok(balance);
    }
}
