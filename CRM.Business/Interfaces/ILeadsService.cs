﻿using CRM.Business.Models.Leads.Requests;
using CRM.Business.Models.Leads.Responses;

namespace CRM.Business.Interfaces;

public interface ILeadsService
{
    Task<(Guid leadId, Guid accountId)> AddLeadAsync(RegisterLeadRequest request);
    Task DeleteLeadByIdAsync(Guid id);
    Task<LeadFullResponse> GetLeadByIdAsync(Guid id);
    Task<List<LeadResponse>> GetLeadsAsync();
    Task<Guid> LoginLeadAsync(LoginLeadRequest request);
    Task UpdateLeadAsync(Guid leadId, UpdateLeadDataRequest request);
    Task UpdateLeadBirthDateAsync(Guid leadId, UpdateLeadBirthDateRequest request);
    Task UpdateLeadPasswordAsync(Guid leadId, UpdateLeadPasswordRequest request);
    Task UpdateLeadStatusAsync(Guid leadId, UpdateLeadStatusRequest request);
}
