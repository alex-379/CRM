namespace CRM.Business.Models.Transactions.Requests;

public class TransactionRequest
{
    public Guid AccountId { get; set; }
    public int Amount { get; set; }
}