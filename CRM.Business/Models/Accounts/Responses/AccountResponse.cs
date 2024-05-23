using CRM.Core.Enums;

namespace CRM.Business.Models.Accounts.Responses;

public class AccountResponse
{
    public Currency Currency { get; set; }
    public AccountStatus Status { get; set; }
}
