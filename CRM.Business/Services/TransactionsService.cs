using CRM.Business.Interfaces;
using CRM.Business.Models.Accounts.Responses;
using CRM.Business.Models.Transactions.Requests;
using CRM.Business.Services.Constants.Exceptions;
using CRM.Core.Enums;
using CRM.Core.Exceptions;

namespace CRM.Business.Services;

public class TransactionsService(IAccountsService accountsService) : ITransactionsService
{
    public async Task<DepositWithdrawRequest> CreateDepositWithdrawRequestTStore(TransactionRequest request)
    {
        var account = await accountsService.GetAccountByIdAsync<AccountForTransactionResponse>(request.AccountId);
        CheckCurrencyForDepositWithdrawTransaction(account);
        var tStoreRequest = new DepositWithdrawRequest()
        {
            AccountId = request.AccountId,
            Currency = account.Currency,
            Amount = request.Amount
        };

        return tStoreRequest;
    }
    
    public async Task<TransferRequest> CreateTransferRequestTStore(CrmTransferRequest request)
    {
        var accountFrom = await accountsService.GetAccountByIdAsync<AccountForTransactionResponse>(request.AccountFromId);
        var accountTo = await accountsService.GetAccountByIdAsync<AccountForTransactionResponse>(request.AccountToId);
        CheckLeadAccountsForTransferTransaction(accountFrom, accountTo);
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
    
    private static void CheckLeadAccountsForTransferTransaction(AccountForTransactionResponse accountFrom, AccountForTransactionResponse accountTo)
    {
        if (accountFrom.LeadId != accountTo.LeadId)
        {
            throw new ValidationException(TransactionsServiceExceptions.AccountNotYour);
        }
        
        if (accountFrom.Currency == accountTo.Currency)
        {
            throw new ValidationException(TransactionsServiceExceptions.AccountsCurrencyEqual);
        }
    }

    private static void CheckCurrencyForDepositWithdrawTransaction(AccountForTransactionResponse account)
    {
        Currency[] allowedCurrenciesForDepositWithdrawTransaction = [Currency.Rub, Currency.Usd];
        var allowedCurrencyNames = allowedCurrenciesForDepositWithdrawTransaction.Select(c => c.ToString()).ToArray();
        if(!allowedCurrenciesForDepositWithdrawTransaction.Contains(account.Currency))
        {
            throw new ValidationException(string.Format(TransactionsServiceExceptions.CurrencyForDepositWithdrawTransaction, string.Join(",", allowedCurrencyNames)));
        }
    }
}