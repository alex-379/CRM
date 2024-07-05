using System.Text.Json;
using CRM.API.Configuration.Extensions;
using CRM.Core.Enums;
using MassTransit;
using Messaging.Shared;
using Serilog;
using ILogger = Serilog.ILogger;

namespace CRM.API.Consumers;

public class SettingsConsumer(IConfiguration configuration, IConfigurationRoot configurationRoot) : IConsumer<ConfigurationMessage>
{
    private readonly ILogger _logger = Log.ForContext<SettingsConsumer>();
    
    public Task Consume(ConsumeContext<ConfigurationMessage> context)
    {
        if (context.Message.ServiceType != ServiceType.Crm)
        {
            return Task.CompletedTask;
        }
        var jsonMessage = JsonSerializer.Serialize(context.Message.Configurations);
        _logger.Information(ConsumersLogs.SettingsConsumer, jsonMessage);
        configuration.UpdateSettingsFromConfigurationManager(context.Message.Configurations);
        configurationRoot.Reload();
        
        return Task.CompletedTask;
    }
}