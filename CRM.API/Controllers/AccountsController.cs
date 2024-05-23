﻿using CRM.Business.Interfaces;
using CRM.Business.Models.Accounts.Requests;
using CRM.Core.Constants;
using CRM.Core.Constants.Logs.API;
using CRM.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Security.Claims;

namespace CRM.API.Controllers;

[Authorize]
[ApiController]
[Route(ControllersRoutes.AccountsController)]
public class AccountsController(IAccountsService accountsService) : Controller
{
    private readonly IAccountsService _accountsService = accountsService;
    private readonly Serilog.ILogger _logger = Log.ForContext<LeadsController>();

    [HttpPost]
    public ActionResult<Guid> RegistrationAccount([FromBody] RegistrationAccountRequest request)
    {
        _logger.Information(AccountsControllerLogs.GetAuthorizedAccount);
        var currentUserId = new Guid(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
        _logger.Information(AccountsControllerLogs.RegistrationAccount, request.Currency, currentUserId);
        var id = _accountsService.AddAccount(currentUserId, request);

        return Created($"{ControllersRoutes.Host}{ControllersRoutes.LeadsController}/{id}", id);
    }

    [Authorize(Roles = nameof(LeadStatus.Administrator))]
    [HttpPatch(ControllersRoutes.Status)]
    public ActionResult UpdateAccountStatus([FromRoute] Guid id, [FromBody] UpdateAccountStatusRequest request)
    {
        _logger.Information(AccountsControllerLogs.UpdateAccountStatus, id);
        _accountsService.UpdateAccountStatus(id, request);

        return NoContent();
    }
}