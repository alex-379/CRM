using CRM.Core.Enums;

namespace CRM.Core.Dtos;

public class AccountDto : IdContainer
{
    public Currency Currency { get; init; }
    public AccountStatus Status { get; set; }
    public LeadDto Lead { get; set; }
}
