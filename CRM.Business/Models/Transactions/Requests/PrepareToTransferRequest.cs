namespace CRM.Business.Models.Transactions.Requests;

public class PrepareToTransferRequest
{
    public Guid AccountFromId { get; set; }
    public Guid AccountToId { get; set; }
    public decimal Amount { get; set; }
}