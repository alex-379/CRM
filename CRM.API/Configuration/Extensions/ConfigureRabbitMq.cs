using CRM.API.Configuration.Exceptions;
using MassTransit;

namespace CRM.API.Configuration.Extensions;

public static class ConfigureRabbitMq
{
    public static void AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((_, cfg) =>
            {
                cfg.Host(configuration[ConfigurationSettings.RabbitMqHost], h =>
                {
                    h.Username(configuration[ConfigurationSettings.RabbitMqPassword] ?? throw new ConfigurationMissingException());
                    h.Password(configuration[ConfigurationSettings.RabbitMqUserName] ?? throw new ConfigurationMissingException());
                });
            });
            
        });
    }
}