﻿using AutoMapper;
using CRM.Business.Models.Accounts.Requests;
using CRM.Business.Models.Accounts.Responses;
using CRM.Core.Dtos;

namespace CRM.Business.Models.Accounts;

public class AccountsMappingProfile : Profile
{
    public AccountsMappingProfile()
    {
        CreateMap<RegisterAccountRequest, AccountDto>();

        CreateMap<AccountDto, AccountResponse>();
        CreateMap<AccountDto, AccountForTransactionResponse>();
        CreateMap<AccountDto, AccountForAuthorizationFilterResponse>()
            .ForMember(d => d.LeadId, o => o.MapFrom(s => s.Lead.Id));
    }
}
