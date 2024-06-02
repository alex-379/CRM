using CRM.Core.Dtos;

namespace CRM.DataLayer.Interfaces;

public interface IAccountsRepository
{
    Task<Guid> AddAccountAsync(AccountDto account);
    Task<AccountDto> GetAccountByIdAsync(Guid id);
    Task UpdateAccountAsync(AccountDto account);
}