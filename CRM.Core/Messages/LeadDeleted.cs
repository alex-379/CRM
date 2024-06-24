namespace Messaging.Shared;

public class LeadDeleted
{
    public Guid Id { get; init; }
    public bool IsDeleted { get; init; }
}