namespace CRM.Business.Services.Constants.Logs;

public static class TransactionsServiceLogs
{
    public const string AddDepositTransaction = "Calling the http-client service for sending a deposit transaction for the account with Id: {accountId} with Currency: {currency}";
    public const string AddWithdrawTransaction = "Calling the http-client service for sending a withdraw transaction for the account with Id: {accountId} with Currency: {currency}";
    public const string AddTransferTransaction = "Calling the http-client service for sending a transfer transaction from the account with Id: {accountIdFrom} to the account with Id: {accountIdTo}";
    public const string GetTransactions = "Calling the http-client service for getting transactions for account with Id: {id}";
    public const string GetBalance= "Calling the http-client service for getting balance for account with Id: {id}";
}