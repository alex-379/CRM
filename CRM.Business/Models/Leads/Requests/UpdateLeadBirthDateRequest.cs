using CRM.Core.Enums;

namespace CRM.Business.Models.Leads.Requests;

public class UpdateLeadBirthDateRequest
{
    public DateOnly BirthDate { get; set; }
}
