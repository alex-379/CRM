using CRM.Core.Enums;

namespace CRM.Core.Dtos;

public class AccountDto : IdContainer
{
    public Currency Currency { get; set; }
    public AccountStatus Status { get; set; }
    public LeadDto Lead { get; set; }
}
