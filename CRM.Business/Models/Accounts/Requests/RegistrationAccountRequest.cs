using CRM.Core.Dtos;
using CRM.Core.Enums;

namespace CRM.Business.Models.Accounts.Requests;

public class RegistrationAccountRequest
{
    public Currency Currency { get; set; }
    public LeadDto Lead { get; set; }
}
