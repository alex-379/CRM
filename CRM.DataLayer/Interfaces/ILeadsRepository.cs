using CRM.Core.Dtos;

namespace CRM.DataLayer.Interfaces;

public interface ILeadsRepository
{
    Task<Guid> AddLeadAsync(LeadDto lead);
    Task<LeadDto> GetLeadByIdAsync(Guid id);
    Task<LeadDto> GetLeadByMailAsync(string mail);
    Task<List<LeadDto>> GetLeadsAsync();
    Task UpdateLeadAsync(LeadDto lead);
}