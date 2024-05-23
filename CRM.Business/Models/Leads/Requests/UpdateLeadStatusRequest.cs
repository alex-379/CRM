using CRM.Core.Enums;

namespace CRM.Business.Models.Leads.Requests;

public class UpdateLeadStatusRequest
{
    public LeadStatus Status { get; set; }
}
