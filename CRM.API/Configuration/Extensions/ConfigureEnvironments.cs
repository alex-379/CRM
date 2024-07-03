using CRM.API.Configuration.Exceptions;

namespace CRM.API.Configuration.Extensions;

public static class ConfigureEnvironments
{
    public static void ReadSettingsFromEnvironment(this IConfiguration configuration)
    {
        configuration.ReadSection(ConfigurationSettings.SecretSettings);
    }
    
    private static void ReadValue(this IConfiguration configuration, IConfigurationSection key)
    { 
        var value = key.Value ?? throw new ConfigurationMissingException(ConfigurationExceptions.ConfigurationKeyNull); 
        var env = configuration[value] ?? throw new ConfigurationMissingException(ConfigurationExceptions.EnvironmentVariablesNotSpecified); 
        key.Value = env;
    }
    
    private static void ReadSection(this IConfiguration configuration, string keySection)
    {
        var section = configuration.GetSection(keySection).GetChildren();
        foreach (var key in section)
        {
            ReadValue(configuration, key);    
        }
    }
}
