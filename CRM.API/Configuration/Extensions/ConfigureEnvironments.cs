using CRM.API.Configuration.Exceptions;

namespace CRM.API.Configuration.Extensions;

public static class ConfigureEnvironments
{
    public static void ReadSettingsFromEnvironment(this IConfiguration configuration)
    {
        var log = configuration.GetSection(ConfigurationSettings.LogPath);
        configuration.ReadValue(log);
        var databaseSettings = configuration.GetSection(ConfigurationSettings.DatabaseSettings).GetChildren();
        configuration.ReadSection(databaseSettings);
        var secretSettings = configuration.GetSection(ConfigurationSettings.SecretSettings).GetChildren();
        configuration.ReadSection(secretSettings);
        var rabbitMqSettings = configuration.GetSection(ConfigurationSettings.RabbitMqSettings).GetChildren();
        configuration.ReadSection(rabbitMqSettings);
    }
    
    private static void ReadValue(this IConfiguration configuration, IConfigurationSection key)
    { 
        var value = key.Value ?? throw new ConfigurationMissingException(ConfigurationExceptions.ConfigurationKeyNull); 
        var env = configuration[value] ?? throw new ConfigurationMissingException(ConfigurationExceptions.EnvironmentVariablesNotSpecified); 
        key.Value = env;
    }
    
    private static void ReadSection(this IConfiguration configuration, IEnumerable<IConfigurationSection> section)
    {
        foreach (var key in section)
        {
            ReadValue(configuration, key);    
        }
    }
}
