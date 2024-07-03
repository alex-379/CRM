using AutoMapper;
using CRM.Business.Interfaces;
using CRM.Business.Models.Accounts.Requests;
using CRM.Business.Services.Constants.Exceptions;
using CRM.Business.Services.Constants.Logs;
using CRM.Core.Dtos;
using CRM.Core.Exceptions;
using CRM.DataLayer.Interfaces;
using Messaging.Shared;
using Serilog;

namespace CRM.Business.Services;

public class AccountsService(IAccountsRepository accountsRepository, ILeadsRepository leadsRepository, IMapper mapper, IMessagesService messagesService)
    : IAccountsService
{
    private readonly ILogger _logger = Log.ForContext<AccountsService>();

    public async Task<Guid> AddAccountAsync(Guid leadId, RegisterAccountRequest request)
    {
        var account = mapper.Map<AccountDto>(request);
        account.Lead = await leadsRepository.GetLeadByIdAsync(leadId)
            ?? throw new NotFoundException(string.Format(LeadsServiceExceptions.NotFoundException, leadId));
        _logger.Information(AccountsServiceLogs.AddAccount, request.Currency);
        account.Id = await accountsRepository.AddAccountAsync(account);
        _logger.Information(AccountsServiceLogs.CompleteAccount, account.Id);
        await messagesService.PublishAsync<AccountCreated, AccountDto>(account);

        return account.Id;
    }
    
    public async Task<T> GetAccountByIdAsync<T>(Guid id)
    {
        _logger.Information(AccountsServiceLogs.GetAccountById, id);
        var account = await accountsRepository.GetAccountByIdAsync(id)
                   ?? throw new NotFoundException(string.Format(AccountsServiceExceptions.NotFoundException, id));
        var accountResponse = mapper.Map<T>(account);

        return accountResponse;
    }

    public async Task UpdateAccountStatusAsync(Guid id, UpdateAccountStatusRequest request)
    {
        _logger.Information(AccountsServiceLogs.CheckAccountById, id);
        var account = await accountsRepository.GetAccountByIdAsync(id)
            ?? throw new NotFoundException(string.Format(AccountsServiceExceptions.NotFoundException, id));
        _logger.Information(AccountsServiceLogs.UpdateAccountStatus, request.Status, id);
        account.Status = request.Status;
        _logger.Information(AccountsServiceLogs.UpdateAccountById, id);
        await accountsRepository.UpdateAccountAsync(account);
        await messagesService.PublishAsync<AccountUpdatedStatus, AccountDto>(account);
    }
}
