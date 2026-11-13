
using LIMS.Domain.Instruments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMS.Infrastructure.Persistence.Configurations;

public sealed class InstrumentMaintenanceConfiguration
    : IEntityTypeConfiguration<InstrumentMaintenance>
{
    public void Configure(EntityTypeBuilder<InstrumentMaintenance> builder)
    {
        builder.ToTable("InstrumentMaintenances");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(x => x.ScheduledByUserId)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.CompletedByUserId)
            .HasMaxLength(150);

        builder.Property(x => x.CompletionNotes)
            .HasMaxLength(2000);

        builder.Property(x => x.ScheduledAt)
            .IsRequired();

        builder.HasIndex(x => x.InstrumentId);

        builder.HasIndex(x => x.ScheduledAt);
    }
}
