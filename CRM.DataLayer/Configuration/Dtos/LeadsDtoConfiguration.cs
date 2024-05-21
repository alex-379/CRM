using CRM.Core.Constants;
using CRM.Core.Dtos;
using CRM.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.DataLayer.Configuration.Dtos;

public class LeadsDtoConfiguration : IEntityTypeConfiguration<LeadDto>
{
    public void Configure(EntityTypeBuilder<LeadDto> builder)
    {
        builder
            .HasKey(d => d.Id);
        builder
            .Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(DatabaseProperties.UserNameLength);
        builder
            .Property(u => u.Mail)
            .IsRequired()
            .HasMaxLength(DatabaseProperties.MailLength);
        builder
            .Property(u => u.Phone)
            .IsRequired()
            .HasMaxLength(DatabaseProperties.PhoneLength);
        builder
            .Property(u => u.Address)
            .IsRequired()
            .HasMaxLength(DatabaseProperties.AddressLength);
        builder
            .Property(u => u.BirthDate)
            .IsRequired()
            .HasMaxLength(DatabaseProperties.BirthDateLength);
        builder
            .Property(u => u.Password)
            .IsRequired()
            .HasMaxLength(DatabaseProperties.PasswordLength);
        builder
            .Property(u => u.Status)
            .HasDefaultValue(LeadStatus.Regular);
        builder
            .Property(u => u.IsDeleted)
            .HasDefaultValue(false);
    }
}
