
using LIMS.Domain.Workflows;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMS.Infrastructure.Persistence.Configurations;

public sealed class WorkflowInstanceConfiguration
    : IEntityTypeConfiguration<WorkflowInstance>
{
    public void Configure(EntityTypeBuilder<WorkflowInstance> builder)
    {
        builder.ToTable("WorkflowInstances");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.WorkflowDefinitionVersion)
            .IsRequired();

        builder.Property(x => x.WorkflowName)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(x => x.ReferenceType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ReferenceId)
            .IsRequired();

        builder.Property(x => x.StartedByUserId)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.StartedAt)
            .IsRequired();

        builder.Property(x => x.CancellationReason)
            .HasMaxLength(2000);

        builder.HasIndex(x => new
        {
            x.ReferenceType,
            x.ReferenceId
        });

        builder.HasIndex(x => x.WorkflowDefinitionId);

        builder.HasIndex(x => x.Status);

        builder.HasMany(x => x.Tasks)
            .WithOne()
            .HasForeignKey(x => x.WorkflowInstanceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Tasks)
            .HasField("_tasks")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
