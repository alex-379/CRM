namespace CRM.Business.Services.Constants.Exceptions;

public static class TransactionsServiceExceptions
{
    public const string AccountNotYour = "You cannot transfer to an account other than your own";
    public const string AccountsCurrencyEqual = "The transfer must be made between different accounts";
    public const string CurrencyForDepositWithdrawTransaction = "You can make a deposit/withdraw transaction in the currencies {0}";
}