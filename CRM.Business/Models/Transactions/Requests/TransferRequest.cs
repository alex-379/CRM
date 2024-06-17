using CRM.Core.Enums;

namespace CRM.Business.Models.Transactions.Requests;

public class TransferRequest
{
    public Guid AccountFromId { get; set; }
    public Guid AccountToId { get; set; }
    public Currency CurrencyFromType { get; set; }
    public Currency CurrencyToType { get; set; }
    public decimal Amount { get; set; }
}