using CRM.API.Configuration.Extensions;
using CRM.Business.Configuration;
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
            builder.Configuration.AddJsonFile(ConfigurationSettings.DefaultConfigurationJson, optional: false, reloadOnChange: true);

            // var dict = new Dictionary<string, string>
            // {
            //     { "Log_ConfigurationManager", "new MyDictClass()" }
            // };
            // builder.Configuration.UpdateSettingsFromConfigurationManager(dict);
            
            
            builder.Configuration.ReadSettingsFromEnvironment();
            await builder.Configuration.ReadSettingsFromConfigurationManager();
            builder.Logging.ClearProviders();
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();

            builder.Host.UseSerilog();
            // Add services to the container.
            builder.Services.ConfigureApiServices(builder.Configuration);
            builder.Services.ConfigureBllServices();
            builder.Services.ConfigureDalServices(builder.Configuration);

            var app = builder.Build();
            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseApp();
            app.MapControllers();
            await app.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex.Message);
        }
        finally
        { 
            await Log.CloseAndFlushAsync();
        }
    }
}
