using AutoMapper;
using CRM.Business.Interfaces;
using CRM.Business.Models.Accounts.Requests;
using CRM.Business.Services.Constants.Exceptions;
using CRM.Business.Services.Constants.Logs;
using CRM.Core;
using CRM.Core.Dtos;
using CRM.Core.Enums;
using CRM.Core.Exceptions;
using CRM.DataLayer.Interfaces;
using Messaging.Shared;
using Serilog;

namespace CRM.Business.Services;

public class AccountsService(IAccountsRepository accountsRepository, ILeadsRepository leadsRepository, IMessagesService messagesService, IMapper mapper)
    : IAccountsService
{
    private readonly ILogger _logger = Log.ForContext<AccountsService>();

    public async Task<Guid> AddAccountAsync(Guid leadId, RegisterAccountRequest request)
    {
        var account = mapper.Map<AccountDto>(request);
        await CheckAccountRegister(leadId, request.Currency);
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
        CheckAccountStatus(account, request.Status);
        _logger.Information(AccountsServiceLogs.UpdateAccountStatus, request.Status, id);
        account.Status = request.Status;
        _logger.Information(AccountsServiceLogs.UpdateAccountById, id);
        await accountsRepository.UpdateAccountAsync(account);
        await messagesService.PublishAsync<AccountUpdatedStatus, AccountDto>(account);
    }

    private static void CheckAccountStatus(AccountDto account, AccountStatus status)
    {
        CheckAccountIsRub(account.Currency, status);
        CheckAccountStatusIsEqual(account.Status, status);
        CheckAccountStatusIsUnknown(status);
    }
    
    private static void CheckAccountIsRub(Currency currency, AccountStatus status)
    {
        if (currency == Currency.Rub && status == AccountStatus.Blocked)
        {
            throw new ValidationException(AccountsServiceExceptions.AccountRubException);
        }
    }
    
    private static void CheckAccountStatusIsEqual(AccountStatus accountStatus, AccountStatus requestStatus)
    {
        if (accountStatus == requestStatus)
        {
            throw new ValidationException(AccountsServiceExceptions.AccountStatusEqual);
        }
    }
    
    private static void CheckAccountStatusIsUnknown(AccountStatus status)
    {
        if (status == AccountStatus.Unknown)
        {
            throw new ValidationException(AccountsServiceExceptions.AccountStatusIsUnknown);
        }
    }
    
    private async Task CheckAccountRegister(Guid leadId, Currency currency)
    {
        var lead = await leadsRepository.GetLeadByIdAsync(leadId);
        CheckAccountCurrency(lead.Accounts, currency);
        CheckAllowedCurrencyByLeadStatus(lead.Status, currency);
    }

    private static void CheckAccountCurrency(List<AccountDto> accounts, Currency currency)
    {
        if (accounts.Select(d => d.Currency).Contains(currency))
        {
            throw new ValidationException(AccountsServiceExceptions.AccountCurrencyContains);
        }
    }
    
    private static void CheckAllowedCurrencyByLeadStatus(LeadStatus status, Currency currency)
    {
        var (allowedCurrenciesForRegularLead, allowedCurrencyNames) = AllowedCurrencies.GetAllowedCurrenciesForRegularLead();
        if (currency == Currency.Unknown)
        {
            throw new ValidationException(AccountsServiceExceptions.CurrencyIsUnknown);
        }
        if (status == LeadStatus.Regular && !allowedCurrenciesForRegularLead.Contains(currency))
        {
            throw new ValidationException(string.Format(AccountsServiceExceptions.CurrencyForRegularLead, string.Join(",", allowedCurrencyNames)));
        }
    }
}
