namespace CRM.API.Controllers.Constants;

public static class Routes
{
    public const string Api = "/api/";
    public const string LeadsController = "leads";
    public const string AccountsController = "accounts";
    public const string TokensController = "tokens";
    public const string TransactionsController = "transactions";
    public const string Id = "{id}";
    public const string Login = "login";
    public const string Login2Fa = "login-2fa";
    public const string LeadPassword = "{id}/password";
    public const string Status = "{id}/status";
    public const string LeadBirthDate = "{id}/birthdate";
    public const string Refresh = "refresh";
    public const string Revoke = "revoke";
    public const string Deposit = "deposit";
    public const string Withdraw = "withdraw";
    public const string Transfer = "transfer";
    public const string Transactions = "{id}/transactions";
    public const string TransactionsTStore = "transactions/{0}";
    public const string Balance = "{id}/balance";
}
