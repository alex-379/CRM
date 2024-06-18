using CRM.Core.Enums;

namespace CRM.Business.Models.Accounts.Responses;

public class AccountResponse
{
    public Guid Id { get; init;  }
    public Currency Currency { get; init; }
    public AccountStatus Status { get; init; }
}
