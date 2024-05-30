using CRM.Core.Enums;

namespace CRM.Business.Models.Transactions.Requests;

public class TransactionRequest
{
    public Guid AccountId { get; set; }
    public Transaction Transaction { get; set; }
    public Currency Currency { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}