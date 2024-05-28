using CRM.Business.Models.Tokens.Requests;
using CRM.Business.Models.Tokens.Responses;
using System.Security.Claims;
using CRM.Core.Dtos;

namespace CRM.Business.Interfaces;

public interface ITokensService
{
    string GenerateAccessToken(LeadDto lead);
    string GenerateRefreshToken();
    AuthenticatedResponse Refresh(RefreshTokenRequest request);
    void Revoke(Guid userId);
}