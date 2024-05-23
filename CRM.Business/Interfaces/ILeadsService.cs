using CRM.Business.Models.Leads.Requests;
using CRM.Business.Models.Leads.Responses;
using CRM.Business.Models.Tokens.Responses;

namespace CRM.Business.Interfaces;

public interface ILeadsService
{
    Guid AddLead(RegistrationLeadRequest request);
    LeadResponse GetLeadById(Guid id);
    AuthenticatedResponse LoginLead(LoginLeadRequest request);
}