
using LIMS.Domain.Requests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMS.Infrastructure.Persistence.Configurations;

public sealed class SampleRequestConfiguration
    : IEntityTypeConfiguration<SampleRequest>
{
    public void Configure(EntityTypeBuilder<SampleRequest> builder)
    {
        builder.ToTable("SampleRequests");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.RequestCode)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(x => x.Description)
            .HasMaxLength(4000);

        builder.Property(x => x.SampleName)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(x => x.SampleDescription)
            .HasMaxLength(2000);

        builder.Property(x => x.RequestedSampleType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.SampleSource)
            .HasMaxLength(500);

        builder.Property(x => x.RequestedByUserId)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.AssignedReviewerUserId)
            .HasMaxLength(150);

        builder.Property(x => x.Priority)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.ReviewComments)
            .HasMaxLength(4000);

        builder.Property(x => x.RejectionReason)
            .HasMaxLength(2000);

        builder.Property(x => x.CancellationReason)
            .HasMaxLength(2000);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt);

        builder.HasIndex(x => x.RequestCode)
            .IsUnique();

        builder.HasIndex(x => x.RequestedByUserId);

        builder.HasIndex(x => x.LaboratoryId);

        builder.HasIndex(x => x.Status);

        builder.HasIndex(x => new
        {
            x.Status,
            x.CreatedAt
        });
    }
}
