namespace CRM.Business.Services.Constants.Logs;

public static class AccountsServiceLogs
{
    public const string AddDefaultAccount = "Calling the repository method \"To create a new default RUB account for lead\"";
    public const string AddAccount = "Calling the repository method \"To create a new account {currency} for lead\"";
    public const string CompleteAccount = "A new lead has been created with ID: {id}";
    public const string BlockAccount = "Blocking account with ID {id}";
    public const string UpdateAccountStatus = "Updating account's status on {status} with ID: {id}";
    public const string UpdateAccountById = "Calling the repository method \"To update the account with ID {id}\"";
    public const string CheckAccountById = "Checking the account is in the database with ID: {id}";
}
