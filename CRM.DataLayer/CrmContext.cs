using CRM.Core.Dtos;
using CRM.DataLayer.Configuration.Extensions;
using Microsoft.EntityFrameworkCore;

namespace CRM.DataLayer;

public class CrmContext(DbContextOptions<CrmContext> options) : DbContext(options)
{
    public virtual DbSet<LeadDto> Leads { get; init; } = default;
    public virtual DbSet<AccountDto> Accounts { get; init; } = default;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsForEntitiesInContext();
    }
}
