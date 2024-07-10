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
using Serilog;

namespace CRM.Business.Services;

public class TransactionsService(IAccountsService accountsService, ILeadsService leadsService, IHttpClientService<TransactionStoreHttpClient> httpClientService) : ITransactionsService
{
    private readonly ILogger _logger = Log.ForContext<TransactionsService>();
    
    public async Task<Guid> AddDepositTransactionAsync(TransactionRequest request)
    {
        var tStoreRequest = await CreateDepositWithdrawRequestTStoreAsync(request);
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, Routes.DepositTStore);
        _logger.Information(TransactionsServiceLogs.AddDepositTransaction, tStoreRequest.AccountId, tStoreRequest.Currency);
        var id = await httpClientService.SendAsync<DepositWithdrawRequest,Guid>(tStoreRequest, requestMessage);

        return id;
    }
    
    public async Task<Guid> AddWithdrawTransactionAsync(TransactionRequest request)
    {
        await CheckBalanceAsync(request.AccountId, request.Amount);
        var tStoreRequest = await CreateDepositWithdrawRequestTStoreAsync(request);
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, Routes.WithdrawTStore);
        _logger.Information(TransactionsServiceLogs.AddWithdrawTransaction, tStoreRequest.AccountId, tStoreRequest.Currency);
        var id = await httpClientService.SendAsync<DepositWithdrawRequest,Guid>(tStoreRequest, requestMessage);
        
        return id;
    }
    
    public async Task<TransferGuidsResponse> AddTransferTransactionAsync(CrmTransferRequest request)
    {
        await CheckBalanceAsync(request.AccountFromId, request.Amount);
        var tStoreRequest = await CreateTransferRequestTStoreAsync(request);
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, Routes.TransferTStore);
        _logger.Information(TransactionsServiceLogs.AddTransferTransaction, tStoreRequest.AccountFromId, tStoreRequest.AccountToId);
        var response = await httpClientService.SendAsync<TransferRequest,TransferGuidsResponse>(tStoreRequest, requestMessage);

        return response;
    }
    
    public async Task<List<TransactionResponse>> GetTransactionsByAccountIdAsync(Guid id)
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
    
    public async Task<AccountBalanceResponse> GetBalanceByAccountIdAsync(Guid id)
    {
        _logger.Information(TransactionsServiceLogs.GetBalance, id);
        var balance = await httpClientService.GetAsync<AccountBalanceResponse>(string.Format(Routes.BalanceByAccountIdTStore, id));
        var account = await accountsService.GetAccountByIdAsync<AccountForTransactionResponse>(balance.AccountId);
        balance.Currency = account.Currency;

        return balance;
    }
    
    private async Task<DepositWithdrawRequest> CreateDepositWithdrawRequestTStoreAsync(TransactionRequest request)
    {
        var account = await accountsService.GetAccountByIdAsync<AccountForTransactionResponse>(request.AccountId);
        CheckCurrencyForDepositWithdrawTransactionAsync(account.Currency);
        var tStoreRequest = new DepositWithdrawRequest
        {
            AccountId = request.AccountId,
            Currency = account.Currency,
            Amount = request.Amount
        };

        return tStoreRequest;
    }
    
    private async Task<TransferRequest> CreateTransferRequestTStoreAsync(CrmTransferRequest request)
    {
        var accountFrom = await accountsService.GetAccountByIdAsync<AccountForTransactionResponse>(request.AccountFromId);
        var accountTo = await accountsService.GetAccountByIdAsync<AccountForTransactionResponse>(request.AccountToId);
        await CheckLeadAccountsForTransferTransactionAsync(accountFrom, accountTo);
        var tStoreRequest = new TransferRequest
        {
            AccountToId = request.AccountToId,
            AccountFromId = request.AccountFromId,
            CurrencyTo = accountTo.Currency,
            CurrencyFrom = accountFrom.Currency,
            Amount = request.Amount
        };

        return tStoreRequest;
    }

    private async Task CheckLeadAccountsForTransferTransactionAsync(AccountForTransactionResponse accountFrom, AccountForTransactionResponse accountTo)
    {
        if (accountFrom.LeadId != accountTo.LeadId)
        {
            throw new ValidationException(TransactionsServiceExceptions.AccountNotYour);
        }
        
        if (accountFrom.Currency == accountTo.Currency)
        {
            throw new ValidationException(TransactionsServiceExceptions.AccountsCurrencyEqual);
        }
        
        await CheckLeadRightsAsync(accountTo.LeadId, accountFrom.Currency, accountTo.Currency);
    }

    private static void CheckCurrencyForDepositWithdrawTransactionAsync(Currency currency)
    {
        var (allowedCurrenciesForDepositWithdrawTransaction, allowedCurrencyNames) = AllowedCurrencies.GetAllowedCurrenciesForDepositWithdrawTransaction();
        if(!allowedCurrenciesForDepositWithdrawTransaction.Contains(currency))
        {
            throw new ValidationException(string.Format(TransactionsServiceExceptions.CurrencyForDepositWithdrawTransaction, string.Join(",", allowedCurrencyNames)));
        }
    }
    
    private async Task CheckBalanceAsync(Guid accountId, decimal amount)
    {
        var balance = (await httpClientService.GetAsync<AccountBalanceResponse>(string.Format(Routes.BalanceByAccountIdTStore, accountId))).Balance;
        if (amount > balance)
        {
            throw new ValidationException(TransactionsServiceExceptions.BalanceNotEnough);
        }
    }

    private async Task CheckLeadRightsAsync(Guid leadId, Currency currencyFrom, Currency currencyTo)
    {
        var (allowedCurrenciesForRegularLead, allowedCurrencyNames) = AllowedCurrencies.GetAllowedCurrenciesForRegularLead();
        var lead = await leadsService.GetLeadByIdAsync(leadId);
        if (lead.Status != LeadStatus.Vip
            && (!allowedCurrenciesForRegularLead.Contains(currencyFrom) && currencyTo != Currency.Rub
                || !allowedCurrenciesForRegularLead.Contains(currencyTo)))
        {
            throw new ValidationException(string.Format(TransactionsServiceExceptions.CurrencyForRegularLead,
                string.Join(",", allowedCurrencyNames)));
        }
    }
}