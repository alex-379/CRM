using CRM.API.Validators.Leads;
using FluentValidation;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

namespace CRM.API.Configuration.Extensions;

public static class CofigureValidation
{
    public static void AddValidation(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation(configuration =>
        {
            configuration.OverrideDefaultResultFactoryWith<ValidationResultFactory>();
        });
        services.AddValidatorsFromAssemblyContaining<RegistrationLeadValidator>();
    }
}
