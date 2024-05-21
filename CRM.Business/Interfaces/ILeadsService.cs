using CRM.Business.Models.Leads.Requests;

namespace CRM.Business.Interfaces;

public interface ILeadsService
{
    Guid AddLead(RegistrationLeadRequest request);
}