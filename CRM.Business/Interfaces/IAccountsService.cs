using CRM.Business.Models.Accounts.Requests;

namespace CRM.Business.Interfaces;

public interface IAccountsService
{
    Task<Guid> AddAccountAsync(Guid leadId, RegisterAccountRequest request);
    Task UpdateAccountStatusAsync(Guid id, UpdateAccountStatusRequest request);
}