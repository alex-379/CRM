using CRM.Core.Dtos;
using CRM.DataLayer.Interfaces;
using CRM.DataLayer.Repositories.Constants.Logs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace CRM.DataLayer.Repositories
{
    public class AccountsRepository(CrmContext context) : BaseRepository(context), IAccountsRepository
    {
        private readonly ILogger _logger = Log.ForContext<LeadsRepository>();

        public async Task<Guid> AddAccountAsync(AccountDto account)
        {
            await _ctx.Accounts.AddAsync(account);
            await _ctx.SaveChangesAsync();
            _logger.Information(AccountsRepositoryLogs.AddAccount, account.Id);

            return account.Id;
        }

        public async Task<AccountDto> GetAccountByIdAsync(Guid id)
        {
            _logger.Information(AccountsRepositoryLogs.GetAccountById, id);
            var account = await _ctx.Accounts
                .AsQueryable()
                .FirstOrDefaultAsync((d => d.Id == id));

            return account;
        }

        public async Task UpdateAccountAsync(AccountDto account)
        {
            _logger.Information(AccountsRepositoryLogs.UpdateAccount, account.Id);
            _ctx.Accounts.Update(account);
            await _ctx.SaveChangesAsync();
        }
    }
}
