
using LIMS.Domain.Instruments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMS.Infrastructure.Persistence.Configurations;

public sealed class InstrumentConfiguration
    : IEntityTypeConfiguration<Instrument>
{
    public void Configure(EntityTypeBuilder<Instrument> builder)
    {
        builder.ToTable("Instruments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.InstrumentCode)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(x => x.Manufacturer)
            .HasMaxLength(200);

        builder.Property(x => x.Model)
            .HasMaxLength(200);

        builder.Property(x => x.SerialNumber)
            .HasMaxLength(150);

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.Location)
            .HasMaxLength(500);

        builder.Property(x => x.Description)
            .HasMaxLength(4000);

        builder.Property(x => x.RegisteredAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt);

        builder.HasIndex(x => new
        {
            x.LaboratoryId,
            x.InstrumentCode
        }).IsUnique();

        builder.HasIndex(x => x.LaboratoryId);

        builder.HasIndex(x => x.Status);

        builder.HasMany(x => x.MaintenanceRecords)
            .WithOne()
            .HasForeignKey(x => x.InstrumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.MaintenanceRecords)
            .HasField("_maintenanceRecords")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
