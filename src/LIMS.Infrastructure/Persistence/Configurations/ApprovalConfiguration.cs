
using LIMS.Domain.Approvals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMS.Infrastructure.Persistence.Configurations;

public sealed class ApprovalConfiguration
    : IEntityTypeConfiguration<Approval>
{
    public void Configure(EntityTypeBuilder<Approval> builder)
    {
        builder.ToTable("Approvals");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.SubjectType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.SubjectId)
            .IsRequired();

        builder.Property(x => x.RequestedByUserId)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(x => x.Description)
            .HasMaxLength(4000);

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.RequiredApprovalCount)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.CancellationReason)
            .HasMaxLength(2000);

        builder.HasIndex(x => new
        {
            x.SubjectType,
            x.SubjectId
        });

        builder.HasIndex(x => x.RequestedByUserId);

        builder.HasIndex(x => x.Status);

        builder.HasMany(x => x.Reviewers)
            .WithOne()
            .HasForeignKey(x => x.ApprovalId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Reviewers)
            .HasField("_reviewers")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(x => x.Decisions)
            .WithOne()
            .HasForeignKey(x => x.ApprovalId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Decisions)
            .HasField("_decisions")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
