using CRM.Business.Configuration.HttpClients;
using CRM.Business.Interfaces;
using CRM.Business.Models.Accounts.Responses;
using CRM.Business.Models.Transactions.Requests;
using CRM.Business.Models.Transactions.Responses;
using CRM.Business.Services.Constants;
using CRM.Business.Services.Constants.Exceptions;
using CRM.Business.Services.Constants.Logs;
using CRM.Core;
using CRM.Core.Enums;
using CRM.Core.Exceptions;
using CRM.DataLayer.Interfaces;
using Serilog;

namespace CRM.Business.Services;

public class TransactionsService(IAccountsService accountsService, ILeadsRepository leadsRepository, IHttpClientService<TransactionStoreHttpClient> httpClientService) : ITransactionsService
{
    private readonly ILogger _logger = Log.ForContext<TransactionsService>();
    public async Task<Guid> AddDepositTransaction(TransactionRequest request)
    {
        var tStoreRequest = await CreateDepositWithdrawRequestTStore(request);
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, Routes.DepositTStore);
        _logger.Information(TransactionsServiceLogs.AddDepositTransaction, tStoreRequest.AccountId, tStoreRequest.Currency);
        var id = await httpClientService.SendAsync<DepositWithdrawRequest,Guid>(tStoreRequest, requestMessage);

        return id;
    }
    
    public async Task<Guid> AddWithdrawTransaction(TransactionRequest request)
    {
        await CheckBalance(request.AccountId, request.Amount);
        var tStoreRequest = await CreateDepositWithdrawRequestTStore(request);
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, Routes.WithdrawTStore);
        _logger.Information(TransactionsServiceLogs.AddWithdrawTransaction, tStoreRequest.AccountId, tStoreRequest.Currency);
        var id = await httpClientService.SendAsync<DepositWithdrawRequest,Guid>(tStoreRequest, requestMessage);
        
        return id;
    }
    
    public async Task<TransferGuidsResponse> AddTransferTransaction(CrmTransferRequest request)
    {
        await CheckBalance(request.AccountFromId, request.Amount);
        var tStoreRequest = await CreateTransferRequestTStore(request);
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, Routes.TransferTStore);
        _logger.Information(TransactionsServiceLogs.AddTransferTransaction, tStoreRequest.AccountFromId, tStoreRequest.AccountToId);
        var response = await httpClientService.SendAsync<TransferRequest,TransferGuidsResponse>(tStoreRequest, requestMessage);

        return response;
    }
    
    public async Task<List<TransactionResponse>> GetTransactionsByAccountId(Guid id)
    {
        _logger.Information(TransactionsServiceLogs.GetTransactions, id);
        var transactions = await httpClientService.GetAsync<List<TransactionResponse>>(string.Format(Routes.TransactionsByAccountIdTStore, id));
        foreach (var transaction in transactions)
        { 
            var account = await accountsService.GetAccountByIdAsync<AccountForTransactionResponse>(transaction.AccountId);
            transaction.Currency = account.Currency;
        }

        return transactions;
    }
    
    public async Task<AccountBalanceResponse> GetBalanceByAccountId(Guid id)
    {
        _logger.Information(TransactionsServiceLogs.GetBalance, id);
        var balance = await httpClientService.GetAsync<AccountBalanceResponse>(string.Format(Routes.BalanceByAccountIdTStore, id));
        var account = await accountsService.GetAccountByIdAsync<AccountForTransactionResponse>(balance.AccountId);
        balance.Currency = account.Currency;

        return balance;
    }
    
    private async Task<DepositWithdrawRequest> CreateDepositWithdrawRequestTStore(TransactionRequest request)
    {
        var account = await accountsService.GetAccountByIdAsync<AccountForTransactionResponse>(request.AccountId);
        CheckCurrencyForDepositWithdrawTransaction(account.Currency);
        var tStoreRequest = new DepositWithdrawRequest()
        {
            AccountId = request.AccountId,
            Currency = account.Currency,
            Amount = request.Amount
        };

        return tStoreRequest;
    }
    
    private async Task<TransferRequest> CreateTransferRequestTStore(CrmTransferRequest request)
    {
        var accountFrom = await accountsService.GetAccountByIdAsync<AccountForTransactionResponse>(request.AccountFromId);
        var accountTo = await accountsService.GetAccountByIdAsync<AccountForTransactionResponse>(request.AccountToId);
        await CheckLeadAccountsForTransferTransaction(accountFrom, accountTo);
        var tStoreRequest = new TransferRequest()
        {
            AccountToId = request.AccountToId,
            AccountFromId = request.AccountFromId,
            CurrencyTo = accountTo.Currency,
            CurrencyFrom = accountFrom.Currency,
            Amount = request.Amount
        };

        return tStoreRequest;
    }

    private async Task CheckLeadAccountsForTransferTransaction(AccountForTransactionResponse accountFrom, AccountForTransactionResponse accountTo)
    {
        if (accountFrom.LeadId != accountTo.LeadId)
        {
            throw new ValidationException(TransactionsServiceExceptions.AccountNotYour);
        }
        
        if (accountFrom.Currency == accountTo.Currency)
        {
            throw new ValidationException(TransactionsServiceExceptions.AccountsCurrencyEqual);
        }
        
        await CheckLeadRights(accountTo.LeadId, accountFrom.Currency, accountTo.Currency);
    }

    private static void CheckCurrencyForDepositWithdrawTransaction(Currency currency)
    {
        var (allowedCurrenciesForDepositWithdrawTransaction, allowedCurrencyNames) = AllowedCurrencies.GetAllowedCurrenciesForDepositWithdrawTransaction();
        if(!allowedCurrenciesForDepositWithdrawTransaction.Contains(currency))
        {
            throw new ValidationException(string.Format(TransactionsServiceExceptions.CurrencyForDepositWithdrawTransaction, string.Join(",", allowedCurrencyNames)));
        }
    }
    
    private async Task CheckBalance(Guid accountId, decimal amount)
    {
        var balance = (await httpClientService.GetAsync<AccountBalanceResponse>(string.Format(Routes.BalanceByAccountIdTStore, accountId))).Balance;
        if (amount > balance)
        {
            throw new ValidationException(TransactionsServiceExceptions.BalanceNotEnough);
        }
    }

    private async Task CheckLeadRights(Guid leadId, Currency currencyFrom, Currency currencyTo)
    {
        var (allowedCurrenciesForRegularLead, allowedCurrencyNames) = AllowedCurrencies.GetAllowedCurrenciesForRegularLead();
        var lead = await leadsRepository.GetLeadByIdAsync(leadId);
        if (lead.Status == LeadStatus.Regular
            )
        {
            if (!allowedCurrenciesForRegularLead.Contains(currencyFrom) && currencyTo != Currency.Rub)
            {
                throw new ValidationException(string.Format(TransactionsServiceExceptions.CurrencyForRegularLead,
                    string.Join(",", allowedCurrencyNames)));
            }

            if (!allowedCurrenciesForRegularLead.Contains(currencyTo))
            {
                throw new ValidationException(string.Format(TransactionsServiceExceptions.CurrencyForRegularLead,
                    string.Join(",", allowedCurrencyNames)));
            }
        }
    }
}