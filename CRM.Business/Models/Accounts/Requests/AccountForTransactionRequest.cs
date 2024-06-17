using CRM.Core.Enums;

namespace CRM.Business.Models.Accounts.Requests;

public class AccountForTransactionRequest
{
    public Guid Id { get; init; }
    public Currency Currency { get; init; }
}