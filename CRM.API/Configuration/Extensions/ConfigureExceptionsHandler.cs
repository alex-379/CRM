namespace CRM.API.Configuration.Extensions;

public static class ConfigureExceptionsHandler
{
    public static void AddExceptionsHandler(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
    }
}
