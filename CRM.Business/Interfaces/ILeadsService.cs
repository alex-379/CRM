using CRM.Business.Models.Leads.Requests;
using CRM.Business.Models.Leads.Responses;
using CRM.Business.Models.Tokens.Responses;

namespace CRM.Business.Interfaces;

public interface ILeadsService
{
    Guid AddLead(RegistrationLeadRequest request);
    void DeleteLeadById(Guid id);
    LeadResponse GetLeadById(Guid id);
    AuthenticatedResponse LoginLead(LoginLeadRequest request);
    void UpdateLead(Guid leadId, UpdateLeadDataRequest request);
    void UpdateLeadBirthDate(Guid leadId, UpdateLeadBirthDateRequest request);
    void UpdateLeadMail(Guid leadId, UpdateLeadMailRequest request);
    void UpdateLeadPassword(Guid leadId, UpdateLeadPasswordRequest request);
    void UpdateLeadStatus(Guid leadId, UpdateLeadStatusRequest request);
}