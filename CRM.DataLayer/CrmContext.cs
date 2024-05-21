using CRM.Core.Dtos;
using CRM.DataLayer.Configuration.Extensions;
using Microsoft.EntityFrameworkCore;

namespace CRM.DataLayer;

public class CrmContext : DbContext
{
    public CrmContext(DbContextOptions<CrmContext> options) : base(options)
    {
    }

    public CrmContext()
    {
    }

    public virtual DbSet<LeadDto> Leads { get; set; } = default;
    public virtual DbSet<AccountDto> Accounts { get; set; } = default;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsForEntitiesInContext();
    }
}
