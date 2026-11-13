
using LIMS.Domain.Workflows;
using LIMS.Domain.Workflows.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMS.Infrastructure.Persistence.Configurations;

public sealed class WorkflowStepDefinitionConfiguration
    : IEntityTypeConfiguration<WorkflowStepDefinition>
{
    public void Configure(EntityTypeBuilder<WorkflowStepDefinition> builder)
    {
        builder.ToTable("WorkflowStepDefinitions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Sequence)
            .IsRequired();

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.IsRequired)
            .IsRequired();

        builder.Property(x => x.AllowReassignment)
            .IsRequired();

        builder.Property(x => x.RequiredRole)
            .HasMaxLength(100);

        builder.HasIndex(x => new
        {
            x.WorkflowDefinitionId,
            x.Sequence
        }).IsUnique();

        builder.HasIndex(x => new
        {
            x.WorkflowDefinitionId,
            x.Code
        }).IsUnique();
    }
}
