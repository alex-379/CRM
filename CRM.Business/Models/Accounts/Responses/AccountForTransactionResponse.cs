using CRM.Core.Enums;

namespace CRM.Business.Models.Accounts.Responses;

public class AccountForTransactionResponse
{
    public Guid Id { get; init; }
    public Currency Currency { get; init; }
}