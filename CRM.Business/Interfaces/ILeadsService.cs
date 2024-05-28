using CRM.Business.Models.Leads.Requests;
using CRM.Business.Models.Leads.Responses;
using CRM.Business.Models.Tokens.Responses;

namespace CRM.Business.Interfaces;

public interface ILeadsService
{
    Guid AddLead(RegisterLeadRequest request);
    void DeleteLeadById(Guid id);
    LeadFullResponse GetLeadById(Guid id);
    List<LeadResponse> GetLeads();
    AuthenticatedResponse LoginLead(LoginLeadRequest request);
    void UpdateLead(Guid leadId, UpdateLeadDataRequest request);
    void UpdateLeadBirthDate(Guid leadId, UpdateLeadBirthDateRequest request);
    void UpdateLeadPassword(Guid leadId, UpdateLeadPasswordRequest request);
    void UpdateLeadStatus(Guid leadId, UpdateLeadStatusRequest request);
}