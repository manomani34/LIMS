
using LIMS.Domain.Protocols;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMS.Infrastructure.Persistence.Configurations;

public sealed class ProtocolConfiguration
    : IEntityTypeConfiguration<Protocol>
{
    public void Configure(EntityTypeBuilder<Protocol> builder)
    {
        builder.ToTable("Protocols");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ProtocolCode)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(x => x.Description)
            .HasMaxLength(4000);

        builder.Property(x => x.Version)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.CreatedByUserId)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.LaboratoryId,
            x.ProtocolCode,
            x.Version
        }).IsUnique();

        builder.HasIndex(x => x.LaboratoryId);

        builder.HasIndex(x => x.Status);

        builder.HasMany(x => x.Steps)
            .WithOne()
            .HasForeignKey(x => x.ProtocolId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Steps)
            .HasField("_steps")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
