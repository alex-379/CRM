using System.Text.Json;
using CRM.Business.Configuration.HttpClients;
using CRM.Business.Services;
using CRM.Core;

namespace CRM.API.Configuration.Extensions;

public static class ConfigureSettings
{
    private const string HostConfigurationServiceForCrm = "https://194.87.210.5:13000/api/configuration?service=1";
    
    public static async Task ReadSettingsFromConfigurationManager(this IConfiguration configuration)
    {
        var handler = new HttpClientHandler()
        {
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
        };
        using var httpClient = new HttpClient(handler);
        var configurationManagerHttpClient = new ConfigurationManagerHttpClient(httpClient);
        var httpClientService = new HttpClientService<ConfigurationManagerHttpClient>(configurationManagerHttpClient);
        var configurationSettings = await httpClientService.GetAsync<Dictionary<string, string>>(HostConfigurationServiceForCrm);
    }
}