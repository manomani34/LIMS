
using LIMS.Domain.Laboratories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMS.Infrastructure.Persistence.Configurations;

public sealed class LaboratoryConfiguration
    : IEntityTypeConfiguration<Laboratory>
{
    public void Configure(EntityTypeBuilder<Laboratory> builder)
    {
        builder.ToTable("Laboratories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
    .IsRequired()
    .HasMaxLength(150);

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt);

        builder.HasIndex(x => x.Code)
            .IsUnique();

        builder.HasMany(x => x.Members)
            .WithOne()
            .HasForeignKey(x => x.LaboratoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Members)
            .HasField("_members")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
