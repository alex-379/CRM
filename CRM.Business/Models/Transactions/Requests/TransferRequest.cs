using CRM.Core.Enums;

namespace CRM.Business.Models.Transactions.Requests;

public class TransferRequest
{
    public Guid AccountFromId { get; init; }
    public Guid AccountToId { get; init; }
    public Currency CurrencyFrom { get; init; }
    public Currency CurrencyTo { get; init; }
    public decimal Amount { get; init; }
}