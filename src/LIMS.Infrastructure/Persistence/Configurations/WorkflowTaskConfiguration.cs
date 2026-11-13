
using LIMS.Domain.Workflows;
using LIMS.Domain.Workflows.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMS.Infrastructure.Persistence.Configurations;

public sealed class WorkflowTaskConfiguration
    : IEntityTypeConfiguration<WorkflowTask>
{
    public void Configure(EntityTypeBuilder<WorkflowTask> builder)
    {
        builder.ToTable("WorkflowTasks");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Sequence)
            .IsRequired();

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.IsRequired)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.AssignedToUserId)
            .HasMaxLength(150);

        builder.Property(x => x.CompletedByUserId)
            .HasMaxLength(150);

        builder.Property(x => x.RejectionReason)
            .HasMaxLength(2000);

        builder.Property(x => x.CompletionComments)
            .HasMaxLength(4000);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.WorkflowInstanceId,
            x.Sequence
        }).IsUnique();

        builder.HasIndex(x => x.AssignedToUserId);

        builder.HasIndex(x => x.Status);

        builder.HasIndex(x => x.DueAt);
    }
}
