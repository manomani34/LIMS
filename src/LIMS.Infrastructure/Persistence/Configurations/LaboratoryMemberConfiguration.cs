
using LIMS.Domain.Laboratories;
using LIMS.Domain.Laboratories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMS.Infrastructure.Persistence.Configurations;

public sealed class LaboratoryMemberConfiguration
    : IEntityTypeConfiguration<LaboratoryMember>
{
    public void Configure(EntityTypeBuilder<LaboratoryMember> builder)
    {
        builder.ToTable("LaboratoryMembers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Role)
            .IsRequired();

        builder.Property(x => x.JoinedAt)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.LaboratoryId,
            x.UserId
        }).IsUnique();

        builder.HasIndex(x => x.UserId);
    }
}
