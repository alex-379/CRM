using CRM.Core.Enums;

namespace CRM.Business.Services;

public static class AllowedCurrencies
{
    private static readonly Currency[] _allowedCurrenciesForRegularLead = [Currency.Rub, Currency.Usd, Currency.Eur];
    private static readonly Currency[]  _allowedCurrenciesForDepositWithdrawTransaction = [Currency.Rub, Currency.Usd];
    
    public static (Currency[] currencies, string[] names) GetAllowedCurrenciesForRegularLead()
    {
        var (allowedCurrenciesForRegularLead, allowedCurrencyNames) = GetAllowedCurrencies(_allowedCurrenciesForRegularLead);

        return (allowedCurrenciesForRegularLead, allowedCurrencyNames);
    }
    
    public static (Currency[] currencies, string[] names) GetAllowedCurrenciesForDepositWithdrawTransaction()
    {
        var (allowedCurrenciesForDepositWithdrawTransaction, allowedCurrencyNames) = GetAllowedCurrencies(_allowedCurrenciesForDepositWithdrawTransaction);

        return (allowedCurrenciesForDepositWithdrawTransaction, allowedCurrencyNames);
    }
    
    private static (Currency[] currencies, string[] names) GetAllowedCurrencies(Currency[] allowedCurrencies)
    {
        var allowedCurrenciesForRegularLead = allowedCurrencies;
        var allowedCurrencyNames = allowedCurrenciesForRegularLead.Select(c => c.ToString()).ToArray();

        return (allowedCurrenciesForRegularLead, allowedCurrencyNames);
    }
}