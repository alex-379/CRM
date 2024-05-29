using CRM.Core.Dtos;

namespace CRM.DataLayer.Interfaces;

public interface IAccountsRepository
{
    Guid AddAccount(AccountDto account);
    AccountDto GetAccountById(Guid id);
    void UpdateAccount(AccountDto account);
}