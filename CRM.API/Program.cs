using CRM.API.Configuration.Extensions;
using CRM.Business.Configuration;
using CRM.Business.Interfaces;
using CRM.DataLayer.Configuration.Extensions;
using Serilog;

namespace CRM.API;

public static class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.ReadSettingsFromEnvironment();
            builder.Logging.ClearProviders();
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();

            builder.Host.UseSerilog();
            // Add services to the container.
            builder.Services.ConfigureApiServices(builder.Configuration);
            builder.Services.ConfigureBllServices();
            builder.Services.ConfigureDalServices(builder.Configuration);

            var provider = builder.Services.BuildServiceProvider();
            try
            {
                await provider.GetRequiredService<IHttpClientTransactionStoreService>()
                    .Execute();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something went wrong: {ex}");
            }

            var app = builder.Build();
            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseApp();
            app.MapControllers();
            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex.Message);
        }
        finally
        { 
            Log.CloseAndFlush();
        }
    }
}
