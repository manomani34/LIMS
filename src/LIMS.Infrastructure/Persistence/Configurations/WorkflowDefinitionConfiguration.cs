
using LIMS.Domain.Workflows;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMS.Infrastructure.Persistence.Configurations;

public sealed class WorkflowDefinitionConfiguration
    : IEntityTypeConfiguration<WorkflowDefinition>
{
    public void Configure(EntityTypeBuilder<WorkflowDefinition> builder)
    {
        builder.ToTable("WorkflowDefinitions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(x => x.Description)
            .HasMaxLength(4000);

        builder.Property(x => x.CreatedByUserId)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.Version)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.Code,
            x.Version
        }).IsUnique();

        builder.HasIndex(x => x.Status);

        builder.HasMany(x => x.Steps)
            .WithOne()
            .HasForeignKey(x => x.WorkflowDefinitionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Steps)
            .HasField("_steps")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
