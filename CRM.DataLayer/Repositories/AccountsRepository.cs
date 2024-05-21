﻿using CRM.Core.Constants.Logs.DataLayer;
using CRM.Core.Dtos;
using CRM.DataLayer.Interfaces;
using Serilog;

namespace CRM.DataLayer.Repositories
{
    public class AccountsRepository(CrmContext context) : BaseRepository(context), IAccountsRepository
    {
        private readonly ILogger _logger = Log.ForContext<LeadsRepository>();

        public Guid AddAccount(AccountDto account)
        {
            _ctx.Accounts.Add(account);
            _ctx.SaveChanges();
            _logger.Information(AccountsRepositoryLogs.AddAccount, account.Id);

            return account.Id;
        }

        public void UpdateAccount(AccountDto account)
        {
            _logger.Information(AccountsRepositoryLogs.UpdateAccount, account.Id);
            _ctx.Accounts.Update(account);
            _ctx.SaveChanges();
        }
    }
}
