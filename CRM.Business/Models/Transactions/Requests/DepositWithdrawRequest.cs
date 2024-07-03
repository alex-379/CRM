using CRM.Core.Enums;

namespace CRM.Business.Models.Transactions.Requests;

public class DepositWithdrawRequest
{
    public Guid AccountId { get; init; }
    public Currency Currency { get; init; }
    public int Amount { get; set; }
}