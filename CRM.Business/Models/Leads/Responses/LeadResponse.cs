using CRM.Business.Models.Accounts.Responses;
using CRM.Core.Enums;

namespace CRM.Business.Models.Leads.Responses;

public class LeadResponse
{
    public string Name { get; set; }
    public string Mail { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public DateOnly BirthDate { get; set; }
    public LeadStatus Status { get; set; }
    public List<AccountResponse> Accounts { get; set; }
}