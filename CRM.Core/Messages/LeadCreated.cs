using CRM.Core.Enums;

namespace Messaging.Shared;

public class LeadCreated
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Mail { get; init; }
    public string Phone { get; init; }
    public string Address { get; init; }
    public DateOnly BirthDate { get; init; }
    public LeadStatus Status { get; init; }
}
