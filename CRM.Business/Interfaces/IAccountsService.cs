using CRM.Business.Models.Accounts.Requests;

namespace CRM.Business.Interfaces;

public interface IAccountsService
{
    Guid AddAccount(RegistrationAccountRequest request);
    void BlockAccount(AccountRequest request);
    void UnblockAccount(AccountRequest request);
}