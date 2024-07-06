namespace CRM.API.Consumers;

public class ConsumersLogs
{
    public const string SettingsConsumer = "Getting current configuration: {jsonMessage} from Configuration Manager";
    public const string LeadsConsumerRegular = "Setting status Regular for all vip leads";
    public const string LeadsConsumerVip = "Setting status Vip for list of {countLeads} leads from LeadStatusUpdater";
}