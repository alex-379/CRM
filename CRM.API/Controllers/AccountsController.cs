using System.Security.Claims;
using CRM.API.Controllers.Constants;
using CRM.API.Controllers.Constants.Logs;
using CRM.Business.Interfaces;
using CRM.Business.Models.Accounts.Requests;
using CRM.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using CRM.Core.Exceptions;

namespace CRM.API.Controllers;

[Authorize]
[ApiController]
[Route(Routes.AccountsController)]
public class AccountsController(IAccountsService accountsService) : Controller
{
    private readonly Serilog.ILogger _logger = Log.ForContext<LeadsController>();

    [HttpPost]
    public async Task<ActionResult<Guid>> RegisterAccountAsync([FromBody] RegisterAccountRequest request)
    {
        _logger.Information(AccountsControllerLogs.GetAuthorizedAccount);
        var currentLeadId = GetCurrentLeadFromClaims(HttpContext.User); 
        _logger.Information(AccountsControllerLogs.RegistrationAccount, request.Currency, currentLeadId);
        var id = await accountsService.AddAccountAsync(currentLeadId, request);

        return Created($"{Routes.Host}{Routes.LeadsController}/{id}", id);
    }
    
    private static Guid GetCurrentLeadFromClaims(ClaimsPrincipal claimsPrincipal)
    {
        var currentLeadId = new Guid(claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier)
                                     ?? throw new NotFoundException(Exceptions.ClaimNotFound));

        return currentLeadId;
    }

    [Authorize(Roles = nameof(LeadStatus.Administrator))]
    [HttpPatch(Routes.Status)]
    public async Task<ActionResult> UpdateAccountStatusAsync([FromRoute] Guid id, [FromBody] UpdateAccountStatusRequest request)
    {
        _logger.Information(AccountsControllerLogs.UpdateAccountStatus, id);
        await accountsService.UpdateAccountStatusAsync(id, request);

        return NoContent();
    }
}
