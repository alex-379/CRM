using CRM.Business.Interfaces;
using CRM.Business.Services;
using CRM.Business.Services.Constants;
using Microsoft.Extensions.DependencyInjection;

namespace CRM.Business.Configuration.HttpClients;

public static class ConfigureHttpClientService
{
    public static void AddHttpClientService(this IServiceCollection services)
    {
        services.AddHttpClient<BaseHttpClient>();
        services.AddScoped<IHttpClientService, HttpClientService>();
    }
}

//HttpClientHandler clientHandler = new HttpClientHandler();
//clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

// Pass the handler to httpclient(from you are calling api)
//HttpClient client = new HttpClient(clientHandler);

// services.AddHttpClient<BaseHttpClient>(client =>
// {
//     // Настраиваем HttpClientHandler
//     var clientHandler = new HttpClientHandler();
//     clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
//
//     // Создаем HttpClient с настроенным HttpClientHandler
//     client = new HttpClient(clientHandler);
//     client.Timeout = new TimeSpan(NumericData.HttpClientTimeoutHour, NumericData.HttpClientTimeoutMin, NumericData.HttpClientTimeoutSec);
//     client.DefaultRequestHeaders.Clear();
// });;