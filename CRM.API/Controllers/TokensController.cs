using CRM.API.Controllers.Logs;
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
    public ActionResult<AuthenticatedResponse> Refresh([FromBody] RefreshTokenRequest request)
    {
        _logger.Information(TokensControllerLogs.Refresh);
        var authenticatedResponse = tokensService.Refresh(request);

        return Ok(authenticatedResponse);
    }
    
    [HttpPost]
    [Route(Routes.Revoke)]
    public ActionResult Revoke([FromBody] Guid id)
    {
        _logger.Information(TokensControllerLogs.Revoke);
        tokensService.Revoke(id);

        return NoContent();
    }
}
