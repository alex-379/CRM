namespace CRM.Business.Services.Constants.Exceptions;

public static class AccountsServiceExceptions
{
    public const string NotFoundException = "Account with Id: {0} not found";
    public const string AccountRubException = "You cannot deactivate a ruble account";
    public const string AccountStatusEqual = "This is the current account status";
    public const string AccountCurrencyContains = "You cannot open two accounts in the same currency";
}
