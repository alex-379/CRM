using CRM.Core.Enums;

namespace CRM.Business.Models.Transactions.Responses;

public class AccountBalanceResponse
{
    public Guid AccountId { get; set; }
    public Currency CurrencyType { get; set; }
    public decimal Balance { get; set; }
}