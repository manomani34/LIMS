
using LIMS.Domain.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMS.Infrastructure.Persistence.Configurations;

public sealed class AnalysisResultConfiguration
    : IEntityTypeConfiguration<AnalysisResult>
{
    public void Configure(EntityTypeBuilder<AnalysisResult> builder)
    {
        builder.ToTable("AnalysisResults");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ResultCode)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(x => x.Description)
            .HasMaxLength(4000);

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.CreatedByUserId)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.SubmittedByUserId)
            .HasMaxLength(150);

        builder.Property(x => x.ReviewedByUserId)
            .HasMaxLength(150);

        builder.Property(x => x.ReviewComments)
            .HasMaxLength(4000);

        builder.Property(x => x.RejectionReason)
            .HasMaxLength(2000);

        builder.Property(x => x.ArchivedByUserId)
            .HasMaxLength(150);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt);

        builder.HasIndex(x => new
        {
            x.LaboratoryId,
            x.ResultCode
        }).IsUnique();

        builder.HasIndex(x => x.LaboratoryId);

        builder.HasIndex(x => x.ExperimentId);

        builder.HasIndex(x => x.SampleId);

        builder.HasIndex(x => x.InstrumentId);

        builder.HasIndex(x => x.Status);

        builder.HasMany(x => x.Entries)
            .WithOne()
            .HasForeignKey(x => x.ResultId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Entries)
            .HasField("_entries")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
