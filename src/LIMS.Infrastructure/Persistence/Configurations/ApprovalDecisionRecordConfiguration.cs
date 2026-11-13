
using LIMS.Domain.Approvals;
using LIMS.Domain.Approvals.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMS.Infrastructure.Persistence.Configurations;

public sealed class ApprovalDecisionRecordConfiguration
    : IEntityTypeConfiguration<ApprovalDecisionRecord>
{
    public void Configure(
        EntityTypeBuilder<ApprovalDecisionRecord> builder)
    {
        builder.ToTable("ApprovalDecisionRecords");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ReviewerUserId)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Decision)
            .IsRequired();

        builder.Property(x => x.Comments)
            .HasMaxLength(4000);

        builder.Property(x => x.DecidedAt)
            .IsRequired();

        builder.HasIndex(x => x.ApprovalId);

        builder.HasIndex(x => x.ReviewerUserId);
    }
}
