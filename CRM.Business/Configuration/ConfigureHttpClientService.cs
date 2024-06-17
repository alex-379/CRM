using CRM.Business.Configuration.HttpClients;
using CRM.Business.Interfaces;
using CRM.Business.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CRM.Business.Configuration;

public static class ConfigureHttpClientService
{
    public static void AddHttpClientService(this IServiceCollection services)
    {
        services.AddHttpClient<TransactionStoreHttpClient>().ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
        });;
        services.AddScoped(typeof(IHttpClientService<>), typeof(HttpClientService<>));
    }
}
