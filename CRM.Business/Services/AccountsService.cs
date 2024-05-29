using AutoMapper;
using CRM.Business.Interfaces;
using CRM.Business.Models.Accounts.Requests;
using CRM.Business.Services.Constants.Exceptions;
using CRM.Business.Services.Constants.Logs;
using CRM.Core.Dtos;
using CRM.Core.Exceptions;
using CRM.DataLayer.Interfaces;
using Serilog;

namespace CRM.Business.Services;

public class AccountsService(IAccountsRepository accountsRepository, ILeadsRepository leadsRepository, IMapper mapper) : IAccountsService
{
    private readonly ILogger _logger = Log.ForContext<AccountsService>();

    public Guid AddAccount(Guid leadId, RegisterAccountRequest request)
    {
        var account = mapper.Map<AccountDto>(request);
        account.Lead = leadsRepository.GetLeadById(leadId)
            ?? throw new NotFoundException(string.Format(LeadsServiceExceptions.NotFoundException, leadId));
        _logger.Information(AccountsServiceLogs.AddAccount, request.Currency);
        account.Id = accountsRepository.AddAccount(account);
        _logger.Information(AccountsServiceLogs.CompleteAccount, account.Id);

        return account.Id;
    }

    public void UpdateAccountStatus(Guid id, UpdateAccountStatusRequest request)
    {
        _logger.Information(AccountsServiceLogs.CheckAccountById, id);
        var account = accountsRepository.GetAccountById(id)
            ?? throw new NotFoundException(string.Format(AccountsServiceExceptions.NotFoundException, id));
        _logger.Information(AccountsServiceLogs.UpdateAccountStatus, request.Status, id);
        account.Status = request.Status;
        _logger.Information(AccountsServiceLogs.UpdateAccountById, id);
        accountsRepository.UpdateAccount(account);
    }
}
