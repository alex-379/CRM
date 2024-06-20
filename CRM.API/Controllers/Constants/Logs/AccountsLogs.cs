namespace CRM.API.Controllers.Constants.Logs;

public static class AccountsLogs
{
    public const string RegisterAccount = "Creating {currency} account for lead with Id: {leadId}";
    public const string UpdateAccountStatus = "Updating account status with Id: {id}";
    public const string GetTransactions = "Getting transactions for account with Id: {id}";
    public const string GetBalance= "Getting balance for account with Id: {id}";
}
