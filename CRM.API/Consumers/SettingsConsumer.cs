using System.Text.Json;
using CRM.API.Configuration.Extensions;
using MassTransit;
using Messaging.Shared;
using Serilog;
using ILogger = Serilog.ILogger;

namespace CRM.API.Consumers;

public class SettingsConsumer(IConfiguration configuration) : IConsumer<ConfigurationMessage>
{
    private readonly ILogger _logger = Log.ForContext<SettingsConsumer>();
    
    public Task Consume(ConsumeContext<ConfigurationMessage> context)
    {
        var jsonMessage = JsonSerializer.Serialize(context.Message.Configurations);
        _logger.Information($"Getting current configuration: {jsonMessage} from Rates Provider");
        configuration.UpdateSettingsFromConfigurationManager(context.Message.Configurations);
        return Task.CompletedTask;
    }
}