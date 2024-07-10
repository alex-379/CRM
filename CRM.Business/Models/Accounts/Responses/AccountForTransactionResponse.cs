using CRM.Core.Enums;

namespace CRM.Business.Models.Accounts.Responses;

public class AccountForTransactionResponse
{
    public Currency Currency { get; set; }
    public Guid LeadId { get; set; }
}