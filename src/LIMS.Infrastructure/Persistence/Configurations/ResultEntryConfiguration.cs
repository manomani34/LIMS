
using LIMS.Domain.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMS.Infrastructure.Persistence.Configurations;

public sealed class ResultEntryConfiguration
    : IEntityTypeConfiguration<ResultEntry>
{
    public void Configure(EntityTypeBuilder<ResultEntry> builder)
    {
        builder.ToTable("ResultEntries");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ParameterName)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(x => x.ValueType)
            .IsRequired();

        builder.Property(x => x.Value)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(x => x.Unit)
            .HasMaxLength(100);

        builder.Property(x => x.Method)
            .HasMaxLength(500);

        builder.Property(x => x.Notes)
            .HasMaxLength(2000);

        builder.HasIndex(x => x.ResultId);
    }
}
