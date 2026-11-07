using LIMS.Domain.Common;
using LIMS.Domain.Laboratories.Entities;
using LIMS.Domain.Laboratories.Enums;
using LIMS.Domain.Laboratories.Events;
using LIMS.Domain.Laboratories.Exceptions;

namespace LIMS.Domain.Laboratories;

public sealed class Laboratory : AggregateRoot<Guid>
{
    private readonly List<LaboratoryMember> _members = [];

    public string Name { get; private set; } = string.Empty;

    public string Code { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public LaboratoryStatus Status { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? UpdatedAt { get; private set; }

    public IReadOnlyCollection<LaboratoryMember> Members =>
        _members.AsReadOnly();

    private Laboratory()
    {
    }

    public static Laboratory Create(
        string name,
        string code,
        string? description,
        DateTimeOffset createdAt)
    {
        var laboratory = new Laboratory
        {
            Id = Guid.NewGuid(),
            Name = NormalizeRequired(name, nameof(name), 150),
            Code = NormalizeCode(code),
            Description = NormalizeOptional(description, 1000),
            Status = LaboratoryStatus.Active,
            CreatedAt = createdAt
        };

        laboratory.AddDomainEvent(
            new LaboratoryCreatedDomainEvent(
                laboratory.Id,
                laboratory.Name,
                createdAt));

        return laboratory;
    }

    public void UpdateInformation(
        string name,
        string code,
        string? description,
        DateTimeOffset updatedAt)
    {
        EnsureNotArchived();

        Name = NormalizeRequired(name, nameof(name), 150);
        Code = NormalizeCode(code);
        Description = NormalizeOptional(description, 1000);
        UpdatedAt = updatedAt;
    }

    public void AddMember(
        string userId,
        LaboratoryMemberRole role,
        DateTimeOffset joinedAt)
    {
        EnsureActive();

        if (string.IsNullOrWhiteSpace(userId))
            throw new LaboratoryDomainException("User identifier is required.");

        var normalizedUserId = userId.Trim();

        if (_members.Any(x =>
                string.Equals(
                    x.UserId,
                    normalizedUserId,
                    StringComparison.OrdinalIgnoreCase)))
        {
            throw new LaboratoryDomainException(
                "The user is already a member of this laboratory.");
        }

        var member = new LaboratoryMember(
            Id,
            normalizedUserId,
            role,
            joinedAt);

        _members.Add(member);

        AddDomainEvent(
            new LaboratoryMemberAddedDomainEvent(
                Id,
                member.UserId,
                member.Role,
                joinedAt));

        UpdatedAt = joinedAt;
    }

    public void ChangeMemberRole(
        string userId,
        LaboratoryMemberRole role,
        DateTimeOffset updatedAt)
    {
        EnsureActive();

        var member = FindMember(userId);
        member.ChangeRole(role);

        UpdatedAt = updatedAt;
    }

    public void RemoveMember(
        string userId,
        DateTimeOffset removedAt)
    {
        EnsureNotArchived();

        var member = FindMember(userId);

        _members.Remove(member);

        AddDomainEvent(
            new LaboratoryMemberRemovedDomainEvent(
                Id,
                member.UserId,
                removedAt));

        UpdatedAt = removedAt;
    }

    public void ChangeStatus(
        LaboratoryStatus newStatus,
        DateTimeOffset changedAt)
    {
        if (!Enum.IsDefined(newStatus))
            throw new LaboratoryDomainException("Invalid laboratory status.");

        if (Status == LaboratoryStatus.Archived)
            throw new LaboratoryDomainException(
                "An archived laboratory cannot change its status.");

        if (Status == newStatus)
            return;

        var previousStatus = Status;

        Status = newStatus;
        UpdatedAt = changedAt;

        AddDomainEvent(
            new LaboratoryStatusChangedDomainEvent(
                Id,
                previousStatus,
                newStatus,
                changedAt));
    }

    private LaboratoryMember FindMember(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new LaboratoryDomainException("User identifier is required.");

        return _members.FirstOrDefault(x =>
                   string.Equals(
                       x.UserId,
                       userId.Trim(),
                       StringComparison.OrdinalIgnoreCase))
               ?? throw new LaboratoryDomainException(
                   "The specified user is not a member of this laboratory.");
    }

    private void EnsureActive()
    {
        if (Status != LaboratoryStatus.Active)
            throw new LaboratoryDomainException(
                "Only active laboratories can perform this operation.");
    }

    private void EnsureNotArchived()
    {
        if (Status == LaboratoryStatus.Archived)
            throw new LaboratoryDomainException(
                "Archived laboratories cannot be modified.");
    }

    private static string NormalizeRequired(
        string value,
        string fieldName,
        int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new LaboratoryDomainException(
                $"{fieldName} is required.");

        var normalized = value.Trim();

        if (normalized.Length > maxLength)
            throw new LaboratoryDomainException(
                $"{fieldName} cannot exceed {maxLength} characters.");

        return normalized;
    }

    private static string NormalizeCode(string code)
    {
        var normalized = NormalizeRequired(code, nameof(code), 30)
            .ToUpperInvariant();

        if (normalized.Any(character =>
                !char.IsLetterOrDigit(character) &&
                character != '-' &&
                character != '_'))
        {
            throw new LaboratoryDomainException(
                "Laboratory code can contain only letters, numbers, hyphens and underscores.");
        }

        return normalized;
    }

    private static string? NormalizeOptional(
        string? value,
        int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var normalized = value.Trim();

        if (normalized.Length > maxLength)
            throw new LaboratoryDomainException(
                $"Description cannot exceed {maxLength} characters.");

        return normalized;
    }
}
