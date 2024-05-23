using CRM.API.Configuration.Filters;
using CRM.Business.Interfaces;
using CRM.Business.Models.Leads.Requests;
using CRM.Business.Models.Leads.Responses;
using CRM.Business.Models.Tokens.Responses;
using CRM.Core.Constants;
using CRM.Core.Constants.Logs.API;
using CRM.Core.Enums;
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
        _logger.Information(LeadsControllerLogs.RegistrationLead, request.Mail);
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

    [Authorize(Roles = nameof(LeadStatus.Administrator))]
    [HttpGet]
    public ActionResult<List<LeadResponse>> GetLeads()
    {
        _logger.Information(LeadsControllerLogs.GetLeads);

        return Ok(_leadsService.GetLeads());
    }

    [AuthorizationFilterByLeadId]
    [HttpGet(ControllersRoutes.Id)]
    public ActionResult<LeadFullResponse> GetLeadById(Guid id)
    {
        _logger.Information(LeadsControllerLogs.GetLeadById, id);

        return Ok(_leadsService.GetLeadById(id));
    }

    [AuthorizationFilterByLeadId]
    [HttpPut(ControllersRoutes.Id)]
    public ActionResult UpdateLeadData([FromRoute] Guid id, [FromBody] UpdateLeadDataRequest request)
    {
        _logger.Information(LeadsControllerLogs.UpdateLeadData, id);
        _leadsService.UpdateLead(id, request);

        return NoContent();
    }

    [AuthorizationFilterByLeadId]
    [HttpDelete(ControllersRoutes.Id)]
    public ActionResult DeleteLeadById(Guid id)
    {
        _logger.Information(LeadsControllerLogs.DeleteLeadById, id);
        _leadsService.DeleteLeadById(id);

        return NoContent();
    }

    [AuthorizationFilterByLeadId]
    [HttpPatch(ControllersRoutes.LeadPassword)]
    public ActionResult UpdateLeadPassword([FromRoute] Guid id, [FromBody] UpdateLeadPasswordRequest request)
    {
        _logger.Information(LeadsControllerLogs.UpdateLeadPassword, id);
        _leadsService.UpdateLeadPassword(id, request);

        return NoContent();
    }

    [AuthorizationFilterByLeadId]
    [HttpPatch(ControllersRoutes.LeadMail)]
    public ActionResult UpdateLeadMail([FromRoute] Guid id, [FromBody] UpdateLeadMailRequest request)
    {
        _logger.Information(LeadsControllerLogs.UpdateLeadMail, id);
        _leadsService.UpdateLeadMail(id, request);

        return NoContent();
    }

    [Authorize(Roles = nameof(LeadStatus.Administrator))]
    [HttpPatch(ControllersRoutes.Status)]
    public ActionResult UpdateLeadStatus([FromRoute] Guid id, [FromBody] UpdateLeadStatusRequest request)
    {
        _logger.Information(LeadsControllerLogs.UpdateLeadStatus, id);
        _leadsService.UpdateLeadStatus(id, request);

        return NoContent();
    }

    [Authorize(Roles = nameof(LeadStatus.Administrator))]
    [HttpPatch(ControllersRoutes.LeadBirthDate)]
    public ActionResult UpdateLeadBirthDate([FromRoute] Guid id, [FromBody] UpdateLeadBirthDateRequest request)
    {
        _logger.Information(LeadsControllerLogs.UpdateLeadBirthDate, id);
        _leadsService.UpdateLeadBirthDate(id, request);

        return NoContent();
    }
}
