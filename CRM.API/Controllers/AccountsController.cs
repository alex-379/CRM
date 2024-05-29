using System.Security.Claims;
using CRM.Business.Interfaces;
using CRM.Business.Models.Accounts.Requests;
using CRM.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using CRM.API.Controllers.Logs;
using CRM.Core.Exceptions;

namespace CRM.API.Controllers;

[Authorize]
[ApiController]
[Route(Routes.AccountsController)]
public class AccountsController(IAccountsService accountsService) : Controller
{
    private readonly Serilog.ILogger _logger = Log.ForContext<LeadsController>();

    [HttpPost]
    public ActionResult<Guid> RegisterAccount([FromBody] RegisterAccountRequest request)
    {
        _logger.Information(AccountsControllerLogs.GetAuthorizedAccount);
        var currentLeadId = GetCurrentLeadFromClaims(HttpContext.User); 
        _logger.Information(AccountsControllerLogs.RegistrationAccount, request.Currency, currentLeadId);
        var id = accountsService.AddAccount(currentLeadId, request);

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
    public ActionResult UpdateAccountStatus([FromRoute] Guid id, [FromBody] UpdateAccountStatusRequest request)
    {
        _logger.Information(AccountsControllerLogs.UpdateAccountStatus, id);
        accountsService.UpdateAccountStatus(id, request);

        return NoContent();
    }
}
