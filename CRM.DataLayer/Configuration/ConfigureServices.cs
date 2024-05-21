using CRM.DataLayer.Interfaces;
using CRM.DataLayer.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CRM.DataLayer.Configuration;

public static class ConfigureServices
{
    public static void ConfigureDalServices(this IServiceCollection services)
    {
        services.AddScoped<ILeadsRepository, LeadsRepository>();
        services.AddScoped<IAccountsRepository, AccountsRepository>();
        services.AddScoped<ITransactionsRepository, TransactionsRepository>();
    }
}
