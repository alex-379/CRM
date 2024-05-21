using CRM.Business.Models.Leads.Requests;
using CRM.Business.Models.Tokens.Responses;

namespace CRM.Business.Interfaces;

public interface ILeadsService
{
    Guid AddLead(RegistrationLeadRequest request);
    AuthenticatedResponse LoginLead(LoginLeadRequest request);
}