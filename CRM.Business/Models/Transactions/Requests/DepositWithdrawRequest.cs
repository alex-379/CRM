using CRM.Core.Enums;

namespace CRM.Business.Models.Transactions.Requests;

public class DepositWithdrawRequest
{
    public Guid AccountId { get; set; }
    public Currency CurrencyType { get; set; }
    public int Amount { get; set; }
}