using CRM.Core.Enums;
using CRM.DataLayer.Configuration.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace CRM.DataLayer.Configuration.Extensions;

public static class ConfigureDataBase
{
    public static void AddDataBases(this IServiceCollection services, IConfiguration configuration)
    {
        var dataSource = ConfigureDataSource(configuration);
        services.AddDbContext<CrmContext>(
            options => options
                .UseNpgsql(dataSource)
                .UseSnakeCaseNamingConvention()
        );
    }
    
    private static NpgsqlDataSource ConfigureDataSource(IConfiguration configuration)
    {
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(configuration[ConfigurationSettings.DatabaseContext]);
        dataSourceBuilder.MapEnum<AccountStatus>();
        dataSourceBuilder.MapEnum<Currency>();
        dataSourceBuilder.MapEnum<LeadStatus>();
        var dataSource = dataSourceBuilder.Build();

        return dataSource;
    }
}
