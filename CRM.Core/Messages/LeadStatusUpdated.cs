using CRM.Core.Enums;

namespace Messaging.Shared;

public class LeadStatusUpdated
{
    public Guid Id { get; init; }
    public LeadStatus Status { get; init; }
}