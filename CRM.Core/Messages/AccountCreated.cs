using CRM.Core.Enums;

namespace Messaging.Shared;

public class AccountCreated
{
    public Guid Id { get; init; }
    public Currency Currency { get; init; }
    public AccountStatus Status { get; init; }
    public Guid LeadId { get; init; }
}