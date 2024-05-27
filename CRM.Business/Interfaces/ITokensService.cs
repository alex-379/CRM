using CRM.Business.Models.Tokens.Requests;
using CRM.Business.Models.Tokens.Responses;
using System.Security.Claims;

namespace CRM.Business.Interfaces;

public interface ITokensService
{
    string GenerateAccessToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
    AuthenticatedResponse Refresh(RefreshTokenRequest request);
    void Revoke(Guid userId);
}