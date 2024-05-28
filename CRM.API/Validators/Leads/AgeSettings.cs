using CRM.Core.Constants;

namespace CRM.API.Validators.Leads;

public static class AgeSettings
{
    public static DateOnly AgeStart { get; } = DateOnly.FromDateTime(DateTime.Today.AddYears(ValidationSettings.Age));
    public static DateOnly AgeEnd { get; } = DateOnly.FromDateTime(DateTime.Today);
}