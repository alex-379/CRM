namespace CRM.Business.Models.Transactions.Requests;

public class CrmTransferRequest
{
    public Guid AccountFromId { get; set; }
    public Guid AccountToId { get; set; }
    public decimal Amount { get; set; }
}