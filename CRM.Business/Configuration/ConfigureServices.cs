using CRM.Business.Interfaces;
using CRM.Business.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CRM.Business.Configuration;

public static class ConfigureServices
{
    public static void ConfigureBllServices(this IServiceCollection services)
    {
        services.AddScoped<ILeadsService, LeadsService>();
        services.AddScoped<IAccountsService, AccountsService>();
        services.AddScoped<ITokensService, TokensService>();
        services.AddHttpClientService();
    }
}
