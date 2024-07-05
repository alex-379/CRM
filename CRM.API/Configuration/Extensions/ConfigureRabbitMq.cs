using CRM.API.Configuration.Exceptions;
using CRM.API.Consumers;
using MassTransit;

namespace CRM.API.Configuration.Extensions;

public static class ConfigureRabbitMq
{
    public static void AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<SettingsConsumer>();
            x.AddConsumer<LeadsConsumer>();
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration[ConfigurationSettings.RabbitMqHost], h =>
                {
                    h.Username(configuration[ConfigurationSettings.RabbitMqPassword] ?? throw new ConfigurationMissingException());
                    h.Password(configuration[ConfigurationSettings.RabbitMqUserName] ?? throw new ConfigurationMissingException());
                });
                cfg.ReceiveEndpoint(ConfigurationSettings.ConfigurationQueueName, e =>
                {
                    e.Bind(ConfigurationSettings.ConfigurationExchangeName, с =>
                    {
                        с.ExchangeType = ConfigurationSettings.ConfigurationExchangeType;
                    });
                    e.ConfigureConsumer<SettingsConsumer>(context);
                });
                cfg.ReceiveEndpoint(ConfigurationSettings.LeadUpdaterQueueName, e =>
                {
                    e.Bind(ConfigurationSettings.LeadUpdaterExchangeName, с =>
                    {
                        с.ExchangeType = ConfigurationSettings.LeadUpdaterExchangeType;
                    });
                    e.ConfigureConsumer<LeadsConsumer>(context);
                });
            });
        });
    }
}
