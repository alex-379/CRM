using Microsoft.EntityFrameworkCore;
using System.Reflection;
using CRM.Core.Enums;

namespace CRM.DataLayer.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyConfigurationsForEntitiesInContext(this ModelBuilder modelBuilder)
    {
        var types = modelBuilder.Model.GetEntityTypes().Select(t => t.ClrType).ToHashSet();

        modelBuilder.ApplyConfigurationsFromAssembly(
            Assembly.GetExecutingAssembly(),
                t => t.GetInterfaces()
                .Any(i => i.IsGenericType
                    && i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)
                    && types.Contains(i.GenericTypeArguments[0]))
                );
    }
    
    public static void ConfigureEnums(this ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresEnum<AccountStatus>();
        modelBuilder.HasPostgresEnum<Currency>();
        modelBuilder.HasPostgresEnum<LeadStatus>();
    }
}
