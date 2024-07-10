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
    
    public async Task Consume(ConsumeContext<LeadsMessage> context)
    {
        _logger.Information(ConsumersLogs.LeadsConsumerRegular);
        await leadsService.SetLeadStatusByStatusAsync(LeadStatus.Vip, LeadStatus.Regular);
        var countLeads = context.Message.Leads.Count;
        _logger.Information(ConsumersLogs.LeadsConsumerVip, countLeads);
        await leadsService.SetLeadStatusByIdAsync(context.Message.Leads, LeadStatus.Vip);
    }
}