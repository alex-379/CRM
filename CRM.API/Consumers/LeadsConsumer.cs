using CRM.Business.Interfaces;
using CRM.Core.Enums;
using MassTransit;
using Messaging.Shared;
using Serilog;
using ILogger = Serilog.ILogger;

namespace CRM.API.Consumers;

public class LeadsConsumer(ILeadsService leadsService) : IConsumer<LeadsMessage>
{
    private readonly ILogger _logger = Log.ForContext<SettingsConsumer>();
    
    public Task Consume(ConsumeContext<LeadsMessage> context)
    {
        _logger.Information(ConsumersLogs.LeadsConsumer);
        leadsService.SetLeadStatusAsync(context.Message.Leads, LeadStatus.Vip);
        
        return Task.CompletedTask;
    }
}