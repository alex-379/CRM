using CRM.Core.Enums;

namespace CRM.Business.Models.Transactions.Responses;

public class FullTransactionResponse
{
    public Guid Id { get; set; }
    public Guid AccountFromId { get; set; }
    public Guid AccountToId { get; set; }
    public TransactionType TransactionType { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}