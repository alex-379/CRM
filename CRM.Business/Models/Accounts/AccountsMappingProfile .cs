using AutoMapper;
using CRM.Business.Models.Accounts.Requests;
using CRM.Core.Dtos;

namespace CRM.Business.Models.Accounts;

public class AccountsMappingProfile : Profile
{
    public AccountsMappingProfile()
    {
        CreateMap<RegistrationAccountRequest, AccountDto>();
    }
}
