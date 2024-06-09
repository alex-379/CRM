using CRM.Business.Interfaces;
using CRM.Business.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CRM.Business.Configuration.HttpClients;

public static class ConfigureHttpClientService
{
    public static void AddHttpClientService(this IServiceCollection services)
    {
        services.AddHttpClient<BaseHttpClient>().ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
        });;
        services.AddScoped<IHttpClientService, HttpClientService>();
    }
}
