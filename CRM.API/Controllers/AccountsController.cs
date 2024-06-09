using CRM.API.Controllers.Constants;
using CRM.API.Controllers.Constants.Logs;
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
public class AccountsController(IAccountsService accountsService, IHttpClientService httpClientService) : Controller
{
    private readonly Serilog.ILogger _logger = Log.ForContext<AccountsController>();

    [HttpPost]
    public async Task<ActionResult<Guid>> RegisterAccountAsync([FromBody] RegisterAccountRequest request)
    {
        _logger.Information(AccountsControllerLogs.GetAuthorizedAccount);
        var currentLeadId = InformationFromClaims.GetCurrentLeadFromClaims(HttpContext.User); 
        _logger.Information(AccountsControllerLogs.RegisterAccount, request.Currency, currentLeadId);
        var id = await accountsService.AddAccountAsync(currentLeadId, request);

        return Created($"{Routes.Host}{Routes.LeadsController}/{id}", id);
    }
    
    [Authorize(Roles = nameof(LeadStatus.Administrator))]
    [HttpPatch(Routes.Status)]
    public async Task<ActionResult> UpdateAccountStatusAsync([FromRoute] Guid id, [FromBody] UpdateAccountStatusRequest request)
    {
        _logger.Information(AccountsControllerLogs.UpdateAccountStatus, id);
        await accountsService.UpdateAccountStatusAsync(id, request);

        return NoContent();
    }
    
    [AllowAnonymous]
    [HttpGet("{id}/balance")]
    public async Task<ActionResult<AccountBalanceResponse>> GetBalanceByAccountId(Guid id)
    {
        _logger.Information($"Getting the account balance by its Id {id}. / Получаем баланс аккаунта по его Id {id}.");
        var balance = await httpClientService.GetAsync<AccountBalanceResponse>();
        return Ok(balance);
    }
}
