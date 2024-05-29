using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CRM.API.Configuration.Exceptions;

namespace CRM.API.Configuration.Extensions;

public static class ConfigureAuthentication
{
    public static void AddAuthenticationService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration[ConfigurationSettings.ValidIssuer],
                ValidAudience = configuration[ConfigurationSettings.ValidAudience],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration[ConfigurationSettings.IssuerSigningKey]
                    ?? throw new ConfigurationMissingException(ConfigurationExceptions.ConfigurationKeyNull)))
            };
        });
    }
}
