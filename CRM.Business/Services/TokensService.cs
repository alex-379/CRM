using CRM.Business.Configuration;
using CRM.Business.Interfaces;
using CRM.Business.Models.Tokens.Requests;
using CRM.Business.Models.Tokens.Responses;
using CRM.Core.Exceptions;
using CRM.DataLayer.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CRM.Business.Services.Constants;
using CRM.Business.Services.Constants.Exceptions;
using CRM.Core.Dtos;

namespace CRM.Business.Services;

public class TokensService(SecretSettings secret, JwtToken jwt, ILeadsRepository leadsRepository) : ITokensService
{
    public (string accessToken, string refreshToken) GenerateTokens(LeadDto lead)
    {
        var accessToken = GenerateAccessToken(lead);
        var refreshToken = GenerateRefreshToken();
        
        return (accessToken, refreshToken);
    }

    private string GenerateAccessToken(LeadDto lead)
    {
        var claims = SetClaims(lead);
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret.SecretToken));
        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha512);
        var tokenOptions = new JwtSecurityToken(
            issuer: jwt.ValidIssuer,
            audience: jwt.ValidAudience,
            claims: claims,
            expires: DateTime.Now.AddDays(jwt.LifeTimeAccessToken),
            signingCredentials: signinCredentials
        );
        
        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

        return tokenString;
    }
    
    private static List<Claim> SetClaims(LeadDto lead) =>
    [
        new Claim(ClaimTypes.NameIdentifier, lead.Id.ToString()),
        new Claim(ClaimTypes.Role, lead.Status.ToString())
    ];

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[NumericData.RandomNumber];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        return Convert.ToBase64String(randomNumber);
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret.SecretToken)),
            ValidateLifetime = false
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken
            || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new UnauthenticatedException(TokensServiceExceptions.UnauthenticatedException);
        }

        return principal;
    }

    public async Task<AuthenticatedResponse> RefreshAsync(RefreshTokenRequest request)
    {
        var principal = GetPrincipalFromExpiredToken(request.AccessToken);
        var leadId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? throw new UnauthenticatedException(TokensServiceExceptions.UnauthenticatedException);
        var lead = await leadsRepository.GetLeadByIdAsync(new Guid(leadId));
        if (lead is null || lead.RefreshToken != request.RefreshToken || lead.RefreshTokenExpiryTime <= DateTime.Now)
        {
            throw new UnauthenticatedException(TokensServiceExceptions.UnauthenticatedException);
        }
        
        var (newAccessToken, newRefreshToken) = UpdateLeadTokens(lead);
        
        return new AuthenticatedResponse()
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
        };
    }
    
    private (string newAccessToken, string newRefreshToken) UpdateLeadTokens(LeadDto lead)
    {
        var (newAccessToken, newRefreshToken) = GenerateTokens(lead);
        lead.RefreshToken = newRefreshToken;
        leadsRepository.UpdateLeadAsync(lead);
        
        return (newAccessToken, newRefreshToken);
    }

    public async Task RevokeAsync(Guid userId)
    {
        var lead = await leadsRepository.GetLeadByIdAsync(userId) ?? throw new NotFoundException(string.Format(LeadsServiceExceptions.NotFoundException, userId));
        lead.RefreshToken = null;
        lead.RefreshTokenExpiryTime = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified);
        await leadsRepository.UpdateLeadAsync(lead);
    }
}
