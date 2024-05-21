using CRM.API.Configuration.Extensions;
using CRM.Business.Configuration;
using CRM.DataLayer.Configuration;
using Serilog;

namespace CRM.Core;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Logging.ClearProviders();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();

            builder.Configuration.ReadSettingsFromEnviroment();
            builder.Host.UseSerilog();
            // Add services to the container.
            builder.Services.ConfigureApiServices(builder.Configuration);
            builder.Services.ConfigureBllServices();
            builder.Services.ConfigureDalServices();

            var app = builder.Build();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

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
