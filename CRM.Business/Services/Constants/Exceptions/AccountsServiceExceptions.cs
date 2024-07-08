namespace CRM.Business.Services.Constants.Exceptions;

public static class AccountsServiceExceptions
{
    public const string NotFoundException = "Account with Id: {0} not found";
    public const string AccountRubException = "You cannot deactivate a ruble account";
}
