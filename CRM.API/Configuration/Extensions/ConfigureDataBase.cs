using CRM.Core.Constants;
using CRM.DataLayer;
using Microsoft.EntityFrameworkCore;

namespace CRM.API.Configuration.Extensions;

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
