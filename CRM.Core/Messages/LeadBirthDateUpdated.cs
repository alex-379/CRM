namespace Messaging.Shared;

public class LeadBirthDateUpdated
{
    public Guid Id { get; init; }
    public DateOnly BirthDate { get; init; }
}