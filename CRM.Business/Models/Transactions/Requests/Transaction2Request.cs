using CRM.Core.Enums;

namespace CRM.Business.Models.Transactions.Requests;

public class Transaction2Request
{
    public Guid AccountId { get; set; }
    public TransactionType TransactionType { get; set; }
    public Currency Currency { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}