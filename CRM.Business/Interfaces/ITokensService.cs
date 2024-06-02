using CRM.Business.Models.Tokens.Requests;
using CRM.Business.Models.Tokens.Responses;
using System.Security.Claims;
using CRM.Core.Dtos;

namespace CRM.Business.Interfaces;

public interface ITokensService
{
    Task<AuthenticatedResponse> RefreshAsync(RefreshTokenRequest request);
    Task RevokeAsync(Guid userId);
}