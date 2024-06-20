using CRM.Core.Enums;

namespace CRM.Business.Models.Transactions.Responses;

public class TransactionWithAccountIdResponse
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public TransactionType TransactionType { get; set; }
    public Currency CurrencyType { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}