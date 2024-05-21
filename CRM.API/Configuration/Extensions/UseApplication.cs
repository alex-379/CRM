using Serilog;

namespace CRM.API.Configuration.Extensions;

public static class UseApplication
{
    public static void UseApp(this WebApplication app)
    {
        app.UseExceptionHandler();
        app.UseSerilogRequestLogging();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
    }
}
