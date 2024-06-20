using CRM.API.Controllers.Constants;
using CRM.API.Controllers.Constants.Logs;
using CRM.Business.Interfaces;
using CRM.Business.Models.Tokens.Requests;
using CRM.Business.Models.Tokens.Responses;
using CRM.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace CRM.API.Controllers;

[Authorize(Roles = nameof(LeadStatus.Administrator))]
[ApiController]
[Route(Routes.TokensController)]
public class TokensController(ITokensService tokensService) : Controller
{
    private readonly Serilog.ILogger _logger = Log.ForContext<TokensController>();

    [HttpPost]
    [Route(Routes.Refresh)]
    public async Task<ActionResult<AuthenticatedResponse>> RefreshAsync([FromBody] RefreshTokenRequest request)
    {
        _logger.Information(TokensLogs.Refresh);
        var authenticatedResponse = await tokensService.RefreshAsync(request);

        return Ok(authenticatedResponse);
    }
    
    [HttpPost]
    [Route(Routes.Revoke)]
    public async Task<ActionResult> RevokeAsync([FromBody] Guid id)
    {
        _logger.Information(TokensLogs.Revoke);
        await tokensService.RevokeAsync(id);

        return NoContent();
    }
}
