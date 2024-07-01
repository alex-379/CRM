using CRM.API.Configuration.Filters;
using CRM.API.Controllers.Constants;
using CRM.API.Controllers.Constants.Logs;
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
    public async Task<ActionResult<(Guid leadId, Guid accountId)>> RegisterLeadAsync([FromBody] RegisterLeadRequest request)
    {
        _logger.Information(LeadsLogs.RegisterLead, request.Mail);
        var lead = await leadsService.AddLeadAsync(request);
        var response = new
        {
            lead.leadId,
            lead.accountId
        };
        
        return Created($"{Routes.Host}{Routes.LeadsController}/{lead.leadId}", response);
    }

    [AllowAnonymous]
    [HttpPost(Routes.Login)]
    public async Task<ActionResult<AuthenticatedResponse>> LoginAsync([FromBody] LoginLeadRequest request)
    {
        _logger.Information(LeadsLogs.Login);
        var authenticatedResponse = await leadsService.LoginLeadAsync(request);

        return Ok(authenticatedResponse);
    }
    
    [AllowAnonymous]
    [HttpPost(Routes.Login2Fa)]
    public async Task<ActionResult<AuthenticatedResponse>> Login2FaAsync([FromBody] LoginLeadRequest request)
    {
        _logger.Information(LeadsLogs.Login2Fa);
        var authenticated2FaResponse = await leadsService.LoginLeadAsync(request);

        return Ok(authenticated2FaResponse);
    }

    [Authorize(Roles = nameof(LeadStatus.Administrator))]
    [HttpGet]
    public async Task<ActionResult<List<LeadResponse>>> GetLeadsAsync()
    {
        _logger.Information(LeadsLogs.GetLeads);
        var leads = await leadsService.GetLeadsAsync();

        return Ok(leads);
    }

    [AuthorizationFilterByLeadId]
    [HttpGet(Routes.Id)]
    public async Task<ActionResult<LeadFullResponse>> GetLeadByIdAsync(Guid id)
    {
        _logger.Information(LeadsLogs.GetLeadById, id);
        var lead = await leadsService.GetLeadByIdAsync(id);

        return Ok(lead);
    }

    [AuthorizationFilterByLeadId]
    [HttpPut(Routes.Id)]
    public async Task<ActionResult> UpdateLeadDataAsync([FromRoute] Guid id, [FromBody] UpdateLeadDataRequest request)
    {
        _logger.Information(LeadsLogs.UpdateLeadData, id);
        await leadsService.UpdateLeadAsync(id, request);

        return NoContent();
    }

    [AuthorizationFilterByLeadId]
    [HttpDelete(Routes.Id)]
    public async Task<ActionResult> DeleteLeadByIdAsync(Guid id)
    {
        _logger.Information(LeadsLogs.DeleteLeadById, id);
        await leadsService.DeleteLeadByIdAsync(id);

        return NoContent();
    }

    [AuthorizationFilterByLeadId]
    [HttpPatch(Routes.LeadPassword)]
    public async Task<ActionResult> UpdateLeadPasswordAsync([FromRoute] Guid id, [FromBody] UpdateLeadPasswordRequest request)
    {
        _logger.Information(LeadsLogs.UpdateLeadPassword, id);
        await leadsService.UpdateLeadPasswordAsync(id, request);

        return NoContent();
    }

    [Authorize(Roles = nameof(LeadStatus.Administrator))]
    [HttpPatch(Routes.Status)]
    public async Task<ActionResult> UpdateLeadStatusAsync([FromRoute] Guid id, [FromBody] UpdateLeadStatusRequest request)
    {
        _logger.Information(LeadsLogs.UpdateLeadStatus, id);
        await leadsService.UpdateLeadStatusAsync(id, request);

        return NoContent();
    }

    [Authorize(Roles = nameof(LeadStatus.Administrator))]
    [HttpPatch(Routes.LeadBirthDate)]
    public async Task<ActionResult> UpdateLeadBirthDateAsync([FromRoute] Guid id, [FromBody] UpdateLeadBirthDateRequest request)
    {
        _logger.Information(LeadsLogs.UpdateLeadBirthDate, id);
        await leadsService.UpdateLeadBirthDateAsync(id, request);

        return NoContent();
    }
}
