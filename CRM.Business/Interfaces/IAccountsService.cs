using CRM.Business.Models.Accounts.Requests;

namespace CRM.Business.Interfaces;

public interface IAccountsService
{
    Guid AddAccount(Guid leadId, RegisterAccountRequest request);
    void UpdateAccountStatus(Guid id, UpdateAccountStatusRequest request);
}