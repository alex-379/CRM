using AutoMapper;
using CRM.Business.Models.Leads.Requests;
using CRM.Business.Models.Leads.Responses;
using CRM.Core.Dtos;
using Messaging.Shared;

namespace CRM.Business.Models.Leads;

public class LeadsMappingProfile : Profile
{
    public LeadsMappingProfile()
    {
        CreateMap<RegisterLeadRequest, LeadDto>();
        CreateMap<LoginLeadRequest, LeadDto>();

        CreateMap<LeadDto, LeadResponse>();
        CreateMap<LeadDto, LeadFullResponse>();
        CreateMap<LeadDto, LeadCreated>();
        CreateMap<LeadDto, LeadUpdated>();
        CreateMap<LeadDto, LeadStatusUpdated>();
        CreateMap<LeadDto, LeadPasswordUpdated>();
        CreateMap<LeadDto, LeadBirthDateUpdated>();
        CreateMap<LeadDto, LeadDeleted>();
    }
}
