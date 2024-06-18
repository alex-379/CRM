using CRM.Core.Enums;

namespace CRM.Business.Models.Transactions.Requests;

public class DepositWithdrawRequest
{
    public Guid AccountId { get; init; }
    public Currency CurrencyType { get; init; }
    public int Amount { get; set; }
}