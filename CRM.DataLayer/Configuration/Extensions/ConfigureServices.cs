using CRM.DataLayer.Interfaces;
using CRM.DataLayer.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CRM.DataLayer.Configuration.Extensions;

public static class ConfigureServices
{
    public static void ConfigureDalServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDataBases(configuration);
        services.AddScoped<ILeadsRepository, LeadsRepository>();
        services.AddScoped<IAccountsRepository, AccountsRepository>();
        services.AddScoped<ITransactionsRepository, TransactionsRepository>();
    }
}
