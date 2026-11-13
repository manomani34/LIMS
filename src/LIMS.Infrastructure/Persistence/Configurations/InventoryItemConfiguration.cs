using LIMS.Domain.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMS.Infrastructure.Persistence.Configurations;

public sealed class InventoryItemConfiguration
    : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
        builder.ToTable("InventoryItems");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ItemCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.CasNumber)
            .HasMaxLength(50);

        builder.Property(x => x.ChemicalFormula)
            .HasMaxLength(100);

        builder.Property(x => x.MolecularWeight)
            .HasMaxLength(50);

        builder.Property(x => x.HazardClassification)
            .HasMaxLength(1000);

        builder.Property(x => x.SafetyDataSheetReference)
            .HasMaxLength(500);

        builder.Property(x => x.DefaultUnit)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(x => x.MinimumStockLevel)
            .HasPrecision(18, 6);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt);

        builder.HasIndex(x => new
        {
            x.LaboratoryId,
            x.ItemCode
        }).IsUnique();

        builder.HasIndex(x => new
        {
            x.LaboratoryId,
            x.Status,
            x.Type
        });

        builder.HasMany(x => x.Lots)
            .WithOne()
            .HasForeignKey("InventoryItemId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Lots)
            .HasField("_lots")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(x => x.Movements)
            .WithOne()
            .HasForeignKey("InventoryItemId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(x => x.Movements)
            .HasField("_movements")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
