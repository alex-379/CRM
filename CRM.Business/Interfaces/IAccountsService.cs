using CRM.Business.Models.Accounts.Requests;
using CRM.Business.Models.Accounts.Responses;

namespace CRM.Business.Interfaces;

public interface IAccountsService
{
    Task<Guid> AddAccountAsync(Guid leadId, RegisterAccountRequest request);
    Task<AccountForTransactionResponse> GetAccountByIdAsync(Guid id);
    Task UpdateAccountStatusAsync(Guid id, UpdateAccountStatusRequest request);
}