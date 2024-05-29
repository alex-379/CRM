using CRM.Business.Configuration;

namespace CRM.API.Configuration.Extensions;

public static class ConfigureServicesFromJson
{
    public static void AddConfigurationServicesFromJson(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped(sp => configuration.GetSection(ConfigurationSettings.SecretSettings)
            .Get<SecretSettings>(options => options.BindNonPublicProperties = true));
        services.AddScoped(sp => configuration.GetSection(ConfigurationSettings.JwtToken)
            .Get<JwtToken>(options => options.BindNonPublicProperties = true));
    }
}
