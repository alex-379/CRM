using CRM.Core.Constants;
using CRM.Core.Constants.Exceptions;
using CRM.Core.Exсeptions;

namespace CRM.API.Configuration.Extensions;

public static class ConfigureEnviroments
{
    public static void ReadSettingsFromEnviroment(this IConfiguration configuration)
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
            var value = key.Value;
            var env = configuration[value] ?? throw new ConfigurationMissingException(ConfigurationExceptions.ConfigurationMissingException);
            key.Value = env;
        }
    }
}
