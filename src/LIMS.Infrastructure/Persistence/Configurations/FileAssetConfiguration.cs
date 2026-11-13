
using LIMS.Domain.Files.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMS.Infrastructure.Persistence.Configurations;

public sealed class FileAssetConfiguration
    : IEntityTypeConfiguration<FileAsset>
{
    public void Configure(EntityTypeBuilder<FileAsset> builder)
    {
        builder.ToTable("FileAssets");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OriginalFileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.StorageKey)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.ContentType)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.FileExtension)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.SizeInBytes)
            .IsRequired();

        builder.Property(x => x.Sha256Checksum)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(x => x.Category)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.UploadedByUserId)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.Property(x => x.QuarantineReason)
            .HasMaxLength(1000);

        builder.Property(x => x.RejectionReason)
            .HasMaxLength(1000);

        builder.Property(x => x.UploadedAt)
            .IsRequired();

        builder.HasIndex(x => x.StorageKey)
            .IsUnique();

        builder.HasIndex(x => x.Sha256Checksum);

        builder.HasIndex(x => x.Status);

        builder.HasIndex(x => x.Category);

        builder.HasIndex(x => x.UploadedByUserId);
    }
}
