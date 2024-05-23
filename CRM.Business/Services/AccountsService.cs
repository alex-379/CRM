using AutoMapper;
using CRM.Business.Interfaces;
using CRM.Business.Models.Accounts.Requests;
using CRM.Core.Constants.Exceptions.Business;
using CRM.Core.Constants.Logs.Business;
using CRM.Core.Dtos;
using CRM.Core.Enums;
using CRM.Core.Exсeptions;
using CRM.DataLayer.Interfaces;
using Serilog;

namespace CRM.Business.Services;

public class AccountsService(IAccountsRepository accountsRepository, IMapper mapper) : IAccountsService
{
    private readonly IAccountsRepository _accountsRepository = accountsRepository;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger _logger = Log.ForContext<AccountsService>();

    public Guid AddAccount(RegistrationAccountRequest request)
    {
        var account = _mapper.Map<AccountDto>(request);
        _logger.Information(AccountsServiceLogs.AddAccount, request.Currency);
        account.Id = _accountsRepository.AddAccount(account);
        _logger.Information(AccountsServiceLogs.CompleteAccount, account.Id);

        return account.Id;
    }

    public void BlockAccount(AccountRequest request)
    {
        var account = _accountsRepository.GetAccountById(request.Id)
            ?? throw new NotFoundException(string.Format(AccountsServiceExceptions.NotFoundException, request.Id));
        _logger.Information(AccountsServiceLogs.BlockAccount, account.Id);
        account.Status = AccountStatus.Block;
        _logger.Information(AccountsServiceLogs.UpdateAccountById, account.Id);
        _accountsRepository.UpdateAccount(account);
    }

    public void UnblockAccount(AccountRequest request)
    {
        var account = _accountsRepository.GetAccountById(request.Id)
            ?? throw new NotFoundException(string.Format(AccountsServiceExceptions.NotFoundException, request.Id));
        _logger.Information(AccountsServiceLogs.BlockAccount, account.Id);
        account.Status = AccountStatus.Active;
        _logger.Information(AccountsServiceLogs.UpdateAccountById, account.Id);
        _accountsRepository.UpdateAccount(account);
    }
}
