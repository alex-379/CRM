using CRM.Business.Interfaces;
using CRM.Business.Models.Tokens.Requests;
using CRM.Business.Models.Tokens.Responses;
using CRM.Core.Constants;
using CRM.Core.Constants.Logs.API;
using CRM.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Security.Claims;

namespace CRM.API.Controllers;

[Route(ControllersRoutes.TokensController)]
[ApiController]
public class TokensController(ITokensService tokensService) : Controller
{
    private readonly ITokensService _tokenService = tokensService;
    private readonly Serilog.ILogger _logger = Log.ForContext<TokensController>();

    [HttpPost]
    [Route(ControllersRoutes.Refresh)]
    public ActionResult<AuthenticatedResponse> Refresh([FromBody] RefreshTokenRequest request)
    {
        _logger.Information(TokensControllerLogs.Refresh);
        var authenticatedResponse = _tokenService.Refresh(request);

        return Ok(authenticatedResponse);
    }

    [Authorize(Roles = nameof(LeadStatus.Administrator))]
    [HttpPost]
    [Route(ControllersRoutes.Revoke)]
    public ActionResult Revoke()
    {
        var currentUserId = OperationsWithUsers.GetCurrentUserFromClaims(HttpContext.User); 
        _logger.Information(TokensControllerLogs.Revoke);
        _tokenService.Revoke(currentUserId);

        return NoContent();
    }
}
