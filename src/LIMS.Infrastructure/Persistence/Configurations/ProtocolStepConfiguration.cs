
using LIMS.Domain.Protocols;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMS.Infrastructure.Persistence.Configurations;

public sealed class ProtocolStepConfiguration
    : IEntityTypeConfiguration<ProtocolStep>
{
    public void Configure(EntityTypeBuilder<ProtocolStep> builder)
    {
        builder.ToTable("ProtocolSteps");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Sequence)
            .IsRequired();

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(x => x.Instruction)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.TargetValue)
            .HasMaxLength(200);

        builder.Property(x => x.TargetUnit)
            .HasMaxLength(100);

        builder.Property(x => x.MinimumValue)
            .HasMaxLength(200);

        builder.Property(x => x.MaximumValue)
            .HasMaxLength(200);

        builder.Property(x => x.Notes)
            .HasMaxLength(2000);

        builder.HasIndex(x => new
        {
            x.ProtocolId,
            x.Sequence
        }).IsUnique();
    }
}
