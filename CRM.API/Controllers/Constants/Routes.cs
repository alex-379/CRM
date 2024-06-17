namespace CRM.API.Controllers.Constants;

public static class Routes
{
    public const string Host = "https://194.87.210.5:10000";
    public const string HostTStore = "https://194.87.210.5:11000";
    public const string LeadsController = "/api/leads";
    public const string AccountsController = "api/accounts";
    public const string TokensController = "api/tokens";
    public const string TransactionsController = "api/transactions";
    public const string Id = "{id}";
    public const string Login = "login";
    public const string LeadPassword = "{id}/password";
    public const string Status = "{id}/status";
    public const string LeadBirthDate = "{id}/birthdate";
    public const string Refresh = "refresh";
    public const string Revoke = "revoke";
    public const string Deposit = "deposit";
    public const string DepositTStore = "transactions/deposit";
    public const string Transactions = "{id}/transactions";
    public const string TransactionsTStore = "accounts/{0}/transactions";
    public const string Balance = "{id}/balance";
    public const string BalanceTStore = "accounts/{0}/balance";
    public const string Withdraw = "withdraw";
    public const string WithdrawTStore = "transactions/withdraw";
    public const string Transfer = "transfer";
    public const string TransferTStore = "transactions/transfer";
}
