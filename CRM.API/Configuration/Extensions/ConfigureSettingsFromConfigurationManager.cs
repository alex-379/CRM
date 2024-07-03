using CRM.API.Configuration.Exceptions;
using CRM.Business.Configuration.HttpClients;
using CRM.Business.Services;

namespace CRM.API.Configuration.Extensions;

public static class ConfigureSettingsFromConfigurationManager
{
    public static async Task ReadSettingsFromConfigurationManager(this IConfiguration configuration)
    {
        var configurationSettings = await GetConfigurationSettings(configuration);
        ReadValue(configuration.GetSection(ConfigurationSettings.LogPath), configurationSettings); 
        configuration.ReadSection(ConfigurationSettings.DatabaseSettings, configurationSettings);
        configuration.ReadSection(ConfigurationSettings.RabbitMqSettings, configurationSettings);
        configuration.ReadSection(ConfigurationSettings.ServicesUrlSettings, configurationSettings);
    }

    private static async Task<Dictionary<string, string>> GetConfigurationSettings(IConfiguration configuration)
    {
        using var httpClient = new HttpClient(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
        });
        var configurationManagerHttpClient = new ConfigurationManagerHttpClient(httpClient);
        var httpClientService = new HttpClientService<ConfigurationManagerHttpClient>(configurationManagerHttpClient, new CancellationTokenSource(ConfigurationSettings.TimeCansel));
        var configurationSettings = await httpClientService.GetAsync<Dictionary<string, string>>(configuration[ConfigurationSettings.ConfigurationServiceUrl]);

        return configurationSettings;
    }

    private static void ReadSection(this IConfiguration configuration, string keySection, Dictionary<string, string> configurationSettings)
    {
        var section = configuration.GetSection(keySection).GetChildren();
        foreach (var key in section)
        {
            ReadValue(key, configurationSettings);    
        }
    }
    
    private static void ReadValue(IConfigurationSection key, Dictionary<string, string> configurationSettings)
    { 
        var value = key.Value ?? throw new ConfigurationMissingException(ConfigurationExceptions.ConfigurationKeyNull); 
        if (!configurationSettings.TryGetValue(value, out var configurationSetting))
        {
            throw new ConfigurationMissingException(ConfigurationExceptions.ConfigurationManagerVariablesNotSpecified);
        }
        key.Value = configurationSetting;
    }
}