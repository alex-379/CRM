using CRM.Core.Enums;

namespace CRM.Business.Models.Accounts.Requests;

public class RegisterAccountRequest
{
    public Currency Currency { get; init; }
}
