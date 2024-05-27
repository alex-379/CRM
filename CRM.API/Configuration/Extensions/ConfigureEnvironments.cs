using CRM.API.Configuration.Exceptions;
using CRM.API.Configuration.Exceptions.Constants;
using CRM.Core.Constants;

namespace CRM.API.Configuration.Extensions;

public static class ConfigureEnvironments
{
    public static void ReadSettingsFromEnvironment(this IConfiguration configuration)
    {
        var secretSettings = configuration.GetSection(ConfigurationSettings.SecretSettings).GetChildren();
        configuration.ReadSection(secretSettings);

        var databaseSettings = configuration.GetSection(ConfigurationSettings.DatabaseSettings).GetChildren();
        configuration.ReadSection(databaseSettings);
    }

    private static void ReadSection(this IConfiguration configuration, IEnumerable<IConfigurationSection> section)
    {
        foreach (var key in section)
        {
            var value = key.Value ?? throw new ConfigurationMissingException(ConfigurationExceptions.ConfigurationKeyNull);
            var env = configuration[value] ?? throw new ConfigurationMissingException(ConfigurationExceptions.EnvironmentVariablesNotSpecified);
            key.Value = env;
        }
    }
}
