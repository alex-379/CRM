using CRM.Business.Configuration;
using CRM.Business.Interfaces;
using CRM.Business.Models.Tokens.Requests;
using CRM.Business.Models.Tokens.Responses;
using CRM.Core.Constants.Exceptions.Business;
using CRM.Core.Exceptions;
using CRM.DataLayer.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CRM.Business.Services;

public class TokensService(SecretSettings secret, JwtToken jwt, ILeadsRepository leadsRepository) : ITokensService
{
    private readonly SecretSettings _secret = secret;
    private readonly JwtToken _jwt = jwt;
    private readonly ILeadsRepository _leadsRepository = leadsRepository;

    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret.SecretToken));
        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha512);

        var tokenOptions = new JwtSecurityToken(
            issuer: _jwt.ValidIssuer,
            audience: _jwt.ValidAudience,
            claims: claims,
            expires: DateTime.Now.AddDays(_jwt.LifeTimeAccessToken),
            signingCredentials: signinCredentials
        );
        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

        return tokenString;
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret.SecretToken)),
            ValidateLifetime = false
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken
            || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new UnauthenticatedException(TokensServiceExceptions.UnauthenticatedException);
        }

        return principal;
    }

    public AuthenticatedResponse Refresh(RefreshTokenRequest request)
    {
        var principal = GetPrincipalFromExpiredToken(request.AccessToken);
        var mail = principal.FindFirst(ClaimTypes.Email).Value;
        var lead = _leadsRepository.GetLeadByMail(mail);
        if (lead is null || lead.RefreshToken != request.RefreshToken || lead.RefreshTokenExpiryTime <= DateTime.Now)
        {
            throw new UnauthenticatedException(TokensServiceExceptions.UnauthenticatedException);
        }

        var newAccessToken = GenerateAccessToken(principal.Claims);
        var newRefreshToken = GenerateRefreshToken();
        lead.RefreshToken = newRefreshToken;
        _leadsRepository.UpdateLead(lead);

        return new AuthenticatedResponse()
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
        };
    }

    public void Revoke(string mail)
    {
        var lead = _leadsRepository.GetLeadByMail(mail) ?? throw new NotFoundException(string.Format(LeadsServiceExceptions.NotFoundExceptionMail, mail));
        lead.RefreshToken = null;
        lead.RefreshTokenExpiryTime = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified);
        _leadsRepository.UpdateLead(lead);
    }
}
