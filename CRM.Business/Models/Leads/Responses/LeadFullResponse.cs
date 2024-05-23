using CRM.Business.Models.Accounts.Responses;
using CRM.Core.Enums;

namespace CRM.Business.Models.Leads.Responses;

public class LeadFullResponse : LeadResponse
{
    public string Address { get; set; }
    public DateOnly BirthDate { get; set; }
    public LeadStatus Status { get; set; }
    public List<AccountResponse> Accounts { get; set; }
}