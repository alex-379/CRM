namespace CRM.API.Controllers.Constants.Logs;

public static class TransactionsLogs
{
    public const string AddDepositTransaction = "Adding a deposit transaction for the account with Id: {accountId} with Currency: {currency}";
    public const string AddWithdrawTransaction = "Adding a withdraw transaction for the account with Id: {accountId} with Currency: {currency}";
    public const string AddTransferTransaction = "Adding a transfer transaction from the account with Id: {accountIdFrom} to the account with Id: {accountIdTo}";
    public const string GetTransaction = "Getting the transaction by Id {id}";
}