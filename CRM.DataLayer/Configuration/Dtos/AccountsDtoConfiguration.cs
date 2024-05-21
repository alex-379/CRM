using CRM.Core.Dtos;
using CRM.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.DataLayer.Configuration.Dtos;

public class AccountsDtoConfiguration : IEntityTypeConfiguration<AccountDto>
{
    public void Configure(EntityTypeBuilder<AccountDto> builder)
    {
        builder
            .HasKey(d => d.Id);
        builder
            .HasOne(a => a.Lead)
            .WithMany(l => l.Accounts);
        builder
            .Property(d => d.Currency)
            .IsRequired();
        builder
            .Property(u => u.Status)
            .HasDefaultValue(AccountStatus.Active);
    }
}
