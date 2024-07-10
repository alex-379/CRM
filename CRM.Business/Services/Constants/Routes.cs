namespace CRM.Business.Services.Constants;

public static class Routes
{
    public const string DepositTStore = "transactions/deposit";
    public const string WithdrawTStore = "transactions/withdraw";
    public const string TransferTStore = "transactions/transfer";
    public const string BalanceByAccountIdTStore = "accounts/{0}/balance";
    public const string TransactionsByAccountIdTStore = "accounts/{0}/transactions";
}
