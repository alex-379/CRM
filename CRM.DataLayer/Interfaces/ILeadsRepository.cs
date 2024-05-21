using CRM.Core.Dtos;

namespace CRM.DataLayer.Interfaces;

public interface ILeadsRepository
{
    Guid AddLead(LeadDto lead);
    LeadDto GetLeadById(Guid id);
    LeadDto GetLeadByMail(string mail);
    void UpdateLead(LeadDto lead);
}