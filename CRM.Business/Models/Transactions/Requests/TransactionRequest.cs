namespace CRM.Business.Models.Transactions.Requests;

public class TransactionRequest
{
    public Guid AccountId { get; init; }
    public int Amount { get; init; }
}