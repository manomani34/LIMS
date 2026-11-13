
using LIMS.Domain.Files.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMS.Infrastructure.Persistence.Configurations;

public sealed class FileReferenceConfiguration
    : IEntityTypeConfiguration<FileReference>
{
    public void Configure(EntityTypeBuilder<FileReference> builder)
    {
        builder.ToTable("FileReferences");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ReferenceType)
            .IsRequired();

        builder.Property(x => x.ReferenceId)
            .IsRequired();

        builder.Property(x => x.LinkedByUserId)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Purpose)
            .HasMaxLength(500);

        builder.Property(x => x.LinkedAt)
            .IsRequired();

        builder.HasOne<FileAsset>()
            .WithMany()
            .HasForeignKey(x => x.FileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.ReferenceType,
            x.ReferenceId
        });

        builder.HasIndex(x => x.FileId);

        builder.HasIndex(x => new
        {
            x.FileId,
            x.ReferenceType,
            x.ReferenceId
        }).IsUnique();
    }
}
