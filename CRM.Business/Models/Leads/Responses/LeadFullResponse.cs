using CRM.Business.Models.Accounts.Responses;
using CRM.Core.Enums;

namespace CRM.Business.Models.Leads.Responses;

public class LeadFullResponse : LeadResponse
{
    public string Address { get; init; }
    public DateOnly BirthDate { get; init; }
    public LeadStatus Status { get; init; }
    public List<AccountResponse> Accounts { get; init; }
}