
using LIMS.Domain.Approvals;
using LIMS.Domain.Approvals.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMS.Infrastructure.Persistence.Configurations;

public sealed class ApprovalReviewerConfiguration
    : IEntityTypeConfiguration<ApprovalReviewer>
{
    public void Configure(EntityTypeBuilder<ApprovalReviewer> builder)
    {
        builder.ToTable("ApprovalReviewers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Sequence)
            .IsRequired();

        builder.Property(x => x.IsRequired)
            .IsRequired();

        builder.Property(x => x.HasActed)
            .IsRequired();

        builder.Property(x => x.AssignedAt)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.ApprovalId,
            x.UserId
        }).IsUnique();

        builder.HasIndex(x => new
        {
            x.ApprovalId,
            x.Sequence
        }).IsUnique();
    }
}
