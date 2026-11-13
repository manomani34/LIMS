
using LIMS.Domain.Experiments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMS.Infrastructure.Persistence.Configurations;

public sealed class ExperimentConfiguration
    : IEntityTypeConfiguration<Experiment>
{
    public void Configure(EntityTypeBuilder<Experiment> builder)
    {
        builder.ToTable("Experiments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ExperimentCode)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(x => x.Description)
            .HasMaxLength(4000);

        builder.Property(x => x.CreatedByUserId)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.AssignedToUserId)
            .HasMaxLength(150);

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt);

        builder.HasIndex(x => new
        {
            x.LaboratoryId,
            x.ExperimentCode
        }).IsUnique();

        builder.HasIndex(x => x.LaboratoryId);

        builder.HasIndex(x => x.SampleId);

        builder.HasIndex(x => x.AssignedToUserId);
    }
}
