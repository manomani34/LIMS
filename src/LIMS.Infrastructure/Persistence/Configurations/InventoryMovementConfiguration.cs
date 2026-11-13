using LIMS.Domain.Inventory.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMS.Infrastructure.Persistence.Configurations;

public sealed class InventoryMovementConfiguration
    : IEntityTypeConfiguration<InventoryMovement>
{
    public void Configure(EntityTypeBuilder<InventoryMovement> builder)
    {
        builder.ToTable("InventoryMovements");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.Quantity)
            .HasPrecision(18, 6);

        builder.Property(x => x.BalanceAfterMovement)
            .HasPrecision(18, 6);

        builder.Property(x => x.Unit)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(x => x.PerformedByUserId)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.ReferenceNumber)
            .HasMaxLength(150);

        builder.Property(x => x.Notes)
            .HasMaxLength(2000);

        builder.Property(x => x.OccurredAt)
            .IsRequired();

        builder.HasOne<InventoryLot>()
            .WithMany()
            .HasForeignKey(x => x.LotId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.LotId);

        builder.HasIndex(x => x.ExperimentId);

        builder.HasIndex(x => x.OccurredAt);
    }
}
