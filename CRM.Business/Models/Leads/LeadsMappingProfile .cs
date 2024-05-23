using AutoMapper;
using CRM.Business.Models.Leads.Requests;
using CRM.Business.Models.Leads.Responses;
using CRM.Core.Dtos;

namespace CRM.Business.Models.Leads;

public class LeadsMappingProfile : Profile
{
    public LeadsMappingProfile()
    {
        CreateMap<RegistrationLeadRequest, LeadDto>();
        CreateMap<LoginLeadRequest, LeadDto>();

        CreateMap<LeadDto, LeadResponse>();
        CreateMap<LeadDto, LeadFullResponse>();
    }
}
