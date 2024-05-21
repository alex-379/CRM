using CRM.Core.Dtos;

namespace CRM.DataLayer.Interfaces;

public interface IAccountsRepository
{
    Guid AddAccount(AccountDto account);
    void UpdateAccount(AccountDto account);
}