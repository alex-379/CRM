using CRM.Core.Enums;

namespace CRM.Business.Models.Accounts.Requests;

public class AccountForTransferRequest
{
    public Currency Currency { get; init; }
}