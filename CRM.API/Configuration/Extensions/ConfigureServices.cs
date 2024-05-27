using CRM.Business.Models.Leads;

namespace CRM.API.Configuration.Extensions;

public static class ConfigureServices
{
    public static void ConfigureApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwagger(configuration);
        services.AddExceptionsHandler();
        services.AddAuthenticationService(configuration);
        services.AddAutoMapper(typeof(LeadsMappingProfile).Assembly);
        services.AddValidation();
        services.AddConfigurationFromJson(configuration);
    }
}
