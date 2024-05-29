using CRM.Core.Enums;

namespace CRM.Business.Models.Accounts.Requests;

public class UpdateAccountStatusRequest
{
    public AccountStatus Status { get; init; }
}
