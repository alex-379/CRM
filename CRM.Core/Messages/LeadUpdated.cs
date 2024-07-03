namespace Messaging.Shared;

public class LeadUpdated
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Phone { get; init; }
    public string Address { get; init; }
}