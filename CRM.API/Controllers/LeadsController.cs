using CRM.API.Configuration.Filters;
using CRM.Business.Interfaces;
using CRM.Business.Models.Leads.Requests;
using CRM.Business.Models.Leads.Responses;
using CRM.Business.Models.Tokens.Responses;
using CRM.Core.Constants;
using CRM.Core.Constants.Logs.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace CRM.API.Controllers;

[Authorize]
[ApiController]
[Route(ControllersRoutes.LeadsController)]
public class LeadsController(ILeadsService leadsService) : Controller
{
    private readonly ILeadsService _leadsService = leadsService;
    private readonly Serilog.ILogger _logger = Log.ForContext<LeadsController>();

    [AllowAnonymous]
    [HttpPost]
    public ActionResult<Guid> RegistrationLead([FromBody] RegistrationLeadRequest request)
    {
        _logger.Information(LeadsControllerLogs.CreateUser, request.Mail);
        var id = _leadsService.AddLead(request);

        return Created($"{ControllersRoutes.Host}{ControllersRoutes.LeadsController}/{id}", id);
    }

    [AllowAnonymous]
    [HttpPost(ControllersRoutes.Login)]
    public ActionResult<AuthenticatedResponse> Login([FromBody] LoginLeadRequest request)
    {
        _logger.Information(LeadsControllerLogs.Login);
        var authenticatedResponse = _leadsService.LoginLead(request);

        return Ok(authenticatedResponse);
    }

    [AuthorizationFilterByUserId]
    [HttpGet(ControllersRoutes.Id)]
    public ActionResult<LeadResponse> GetLeadById(Guid id)
    {
        _logger.Information(LeadsControllerLogs.GetLeadById, id);

        return Ok(_leadsService.GetLeadById(id));
    }
}
