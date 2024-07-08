using System.Security.Claims;
using CRM.API.Configuration.Filters;
using CRM.API.Controllers.Constants;
using CRM.API.Controllers.Constants.Logs;
using CRM.Business.Configuration;
using CRM.Business.Interfaces;
using CRM.Business.Models.Accounts.Requests;
using CRM.Business.Models.Accounts.Responses;
using CRM.Business.Models.Transactions.Responses;
using CRM.Core.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace CRM.API.Controllers;

[Authorize]
[ApiController]
[Route($"{Routes.Api}{Routes.AccountsController}")]
public class AccountsController(IAccountsService accountsService, ITransactionsService transactionsService, ServicesUrlSettings servicesUrlSettings) : Controller
{
    private readonly Serilog.ILogger _logger = Log.ForContext<AccountsController>();

    [HttpPost]
    public async Task<ActionResult<Guid>> RegisterAccountAsync([FromBody] RegisterAccountRequest request)
    {
        _logger.Information(LeadsLogs.GetAuthorizedLead);
        var currentLeadId = GetCurrentLeadFromClaims(HttpContext.User); 
        _logger.Information(AccountsLogs.RegisterAccount, request.Currency, currentLeadId);
        var id = await accountsService.AddAccountAsync(currentLeadId, request);

        return Created($"{servicesUrlSettings.Crm}{Routes.LeadsController}/{id}", id);
    }
    
    [AuthorizationFilterByAccountId]
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
        var transactions = await transactionsService.GetTransactionsByAccountId(id);

        return Ok(transactions);
    }
    
    [AuthorizationFilterByAccountId]
    [HttpGet(Routes.Balance)]
    public async Task<ActionResult<AccountBalanceResponse>> GetBalanceByAccountId(Guid id)
    {
        _logger.Information(AccountsLogs.GetBalance, id);
        var balance = await transactionsService.GetBalanceByAccountId(id);

        return Ok(balance);
    }
        
    private static Guid GetCurrentLeadFromClaims(ClaimsPrincipal claimsPrincipal)
    {
        var currentLeadId = new Guid(claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier)
                                     ?? throw new NotFoundException(Exceptions.ClaimNotFound));

        return currentLeadId;
    }
}
