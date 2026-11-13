
using LIMS.Domain.Samples;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMS.Infrastructure.Persistence.Configurations;

public sealed class SampleConfiguration
    : IEntityTypeConfiguration<Sample>
{
    public void Configure(EntityTypeBuilder<Sample> builder)
    {
        builder.ToTable("Samples");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.SampleCode)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(x => x.Description)
            .HasMaxLength(4000);

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.Priority)
            .IsRequired();

        builder.Property(x => x.Source)
            .HasMaxLength(500);

        builder.Property(x => x.AssignedToUserId)
            .HasMaxLength(150);

        builder.Property(x => x.RegisteredByUserId)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.RegisteredAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt);

        builder.Property(x => x.RejectionReason)
            .HasMaxLength(2000);

        builder.HasIndex(x => new
        {
            x.LaboratoryId,
            x.SampleCode
        }).IsUnique();

        builder.HasIndex(x => x.LaboratoryId);

        builder.HasIndex(x => x.AssignedToUserId);

        builder.HasIndex(x => x.Status);
    }
}
