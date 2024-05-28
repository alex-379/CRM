using CRM.DataLayer.Configuration.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CRM.DataLayer.Configuration.Extensions;

public static class ConfigureDataBase
{
    public static void AddDataBases(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CrmContext>(
            options => options
                .UseNpgsql(configuration[ConfigurationSettings.DatabaseContext])
                .UseSnakeCaseNamingConvention()
        );
    }
}
