using CRM.API.Configuration.Filters;
using CRM.API.Controllers.Logs;
using CRM.Business.Interfaces;
using CRM.Business.Models.Leads.Requests;
using CRM.Business.Models.Leads.Responses;
using CRM.Business.Models.Tokens.Responses;
using CRM.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace CRM.API.Controllers;

[Authorize]
[ApiController]
[Route(Routes.LeadsController)]
public class LeadsController(ILeadsService leadsService) : Controller
{
    private readonly Serilog.ILogger _logger = Log.ForContext<LeadsController>();

    [AllowAnonymous]
    [HttpPost]
    public ActionResult<Guid> RegisterLead([FromBody] RegisterLeadRequest request)
    {
        _logger.Information(LeadsControllerLogs.RegistrationLead, request.Mail);
        var id = leadsService.AddLeadAsync(request);

        return Created($"{Routes.Host}{Routes.LeadsController}/{id}", id);
    }

    [AllowAnonymous]
    [HttpPost(Routes.Login)]
    public ActionResult<AuthenticatedResponse> Login([FromBody] LoginLeadRequest request)
    {
        _logger.Information(LeadsControllerLogs.Login);
        var authenticatedResponse = leadsService.LoginLeadAsync(request);

        return Ok(authenticatedResponse);
    }

    [Authorize(Roles = nameof(LeadStatus.Administrator))]
    [HttpGet]
    public ActionResult<List<LeadResponse>> GetLeads()
    {
        _logger.Information(LeadsControllerLogs.GetLeads);

        return Ok(leadsService.GetLeadsAsync());
    }

    [AuthorizationFilterByLeadId]
    [HttpGet(Routes.Id)]
    public ActionResult<LeadFullResponse> GetLeadById(Guid id)
    {
        _logger.Information(LeadsControllerLogs.GetLeadById, id);

        return Ok(leadsService.GetLeadByIdAsync(id));
    }

    [AuthorizationFilterByLeadId]
    [HttpPut(Routes.Id)]
    public ActionResult UpdateLeadData([FromRoute] Guid id, [FromBody] UpdateLeadDataRequest request)
    {
        _logger.Information(LeadsControllerLogs.UpdateLeadData, id);
        leadsService.UpdateLeadAsync(id, request);

        return NoContent();
    }

    [AuthorizationFilterByLeadId]
    [HttpDelete(Routes.Id)]
    public ActionResult DeleteLeadById(Guid id)
    {
        _logger.Information(LeadsControllerLogs.DeleteLeadById, id);
        leadsService.DeleteLeadByIdAsync(id);

        return NoContent();
    }

    [AuthorizationFilterByLeadId]
    [HttpPatch(Routes.LeadPassword)]
    public ActionResult UpdateLeadPassword([FromRoute] Guid id, [FromBody] UpdateLeadPasswordRequest request)
    {
        _logger.Information(LeadsControllerLogs.UpdateLeadPassword, id);
        leadsService.UpdateLeadPasswordAsync(id, request);

        return NoContent();
    }

    [Authorize(Roles = nameof(LeadStatus.Administrator))]
    [HttpPatch(Routes.Status)]
    public ActionResult UpdateLeadStatus([FromRoute] Guid id, [FromBody] UpdateLeadStatusRequest request)
    {
        _logger.Information(LeadsControllerLogs.UpdateLeadStatus, id);
        leadsService.UpdateLeadStatusAsync(id, request);

        return NoContent();
    }

    [Authorize(Roles = nameof(LeadStatus.Administrator))]
    [HttpPatch(Routes.LeadBirthDate)]
    public ActionResult UpdateLeadBirthDate([FromRoute] Guid id, [FromBody] UpdateLeadBirthDateRequest request)
    {
        _logger.Information(LeadsControllerLogs.UpdateLeadBirthDate, id);
        leadsService.UpdateLeadBirthDateAsync(id, request);

        return NoContent();
    }
}
