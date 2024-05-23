using CRM.Business.Models.Accounts.Requests;

namespace CRM.Business.Interfaces;

public interface IAccountsService
{
    Guid AddAccount(Guid leadId, RegistrationAccountRequest request);
    void UpdateAccountStatus(Guid id, UpdateAccountStatusRequest request);
}