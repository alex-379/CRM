using CRM.Core.Enums;

namespace CRM.Business.Models.Transactions.Requests;

public class TransferRequest
{
    public Guid AccountFromId { get; init; }
    public Guid AccountToId { get; init; }
    public Currency CurrencyFromType { get; init; }
    public Currency CurrencyToType { get; init; }
    public decimal Amount { get; init; }
}