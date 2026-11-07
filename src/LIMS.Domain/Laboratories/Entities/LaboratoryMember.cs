using LIMS.Domain.Laboratories.Enums;
using LIMS.Domain.Laboratories.Exceptions;

namespace LIMS.Domain.Laboratories.Entities;

public sealed class LaboratoryMember
{
    public Guid Id { get; private set; }

    public Guid LaboratoryId { get; private set; }

    public string UserId { get; private set; } = string.Empty;

    public LaboratoryMemberRole Role { get; private set; }

    public DateTimeOffset JoinedAt { get; private set; }

    private LaboratoryMember()
    {
    }

    internal LaboratoryMember(
        Guid laboratoryId,
        string userId,
        LaboratoryMemberRole role,
        DateTimeOffset joinedAt)
    {
        if (laboratoryId == Guid.Empty)
            throw new LaboratoryDomainException("Laboratory identifier is required.");

        if (string.IsNullOrWhiteSpace(userId))
            throw new LaboratoryDomainException("User identifier is required.");

        if (!Enum.IsDefined(role))
            throw new LaboratoryDomainException("Invalid laboratory member role.");

        Id = Guid.NewGuid();
        LaboratoryId = laboratoryId;
        UserId = userId.Trim();
        Role = role;
        JoinedAt = joinedAt;
    }

    internal void ChangeRole(LaboratoryMemberRole role)
    {
        if (!Enum.IsDefined(role))
            throw new LaboratoryDomainException("Invalid laboratory member role.");

        Role = role;
    }
}
