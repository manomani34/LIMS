using LIMS.Domain.Inventory.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMS.Infrastructure.Persistence.Configurations;

public sealed class InventoryLotConfiguration
    : IEntityTypeConfiguration<InventoryLot>
{
    public void Configure(EntityTypeBuilder<InventoryLot> builder)
    {
        builder.ToTable("InventoryLots");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.LotNumber)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Supplier)
            .HasMaxLength(200);

        builder.Property(x => x.Manufacturer)
            .HasMaxLength(200);

        builder.Property(x => x.InitialQuantity)
            .HasPrecision(18, 6);

        builder.Property(x => x.AvailableQuantity)
            .HasPrecision(18, 6);

        builder.Property(x => x.Unit)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(x => x.MinimumStockLevel)
            .HasPrecision(18, 6);

        builder.Property(x => x.StorageLocation)
            .HasMaxLength(200);

        builder.Property(x => x.StorageConditions)
            .HasMaxLength(1000);

        builder.Property(x => x.CertificateOfAnalysisNumber)
            .HasMaxLength(150);

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.ReceivedAt)
            .IsRequired();

        builder.HasIndex("InventoryItemId", nameof(InventoryLot.LotNumber))
            .IsUnique();

        builder.HasIndex(x => x.ExpiryDate);

        builder.HasIndex(x => x.Status);
    }
}
