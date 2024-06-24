using CRM.Core.Enums;

namespace Messaging.Shared;

public class AccountUpdatedStatus
{
    public Guid Id { get; init; }
    public AccountStatus Status { get; init; }
}