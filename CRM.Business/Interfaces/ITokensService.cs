using CRM.Business.Models.Tokens.Requests;
using CRM.Business.Models.Tokens.Responses;
using CRM.Core.Dtos;

namespace CRM.Business.Interfaces;

public interface ITokensService
{
    Task<Authenticated2FaResponse> RefreshAsync(RefreshTokenRequest request);
    Task RevokeAsync(Guid userId);
    (string accessToken, string refreshToken) GenerateTokens(LeadDto lead);
}