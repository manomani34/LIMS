
namespace LIMS.Contracts.Laboratories.Responses;

public sealed record LaboratoryResponse(
    Guid Id,
    string Name,
    string Code,
    string? Description,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    int MemberCount);

public sealed record LaboratoryMemberResponse(
    Guid Id,
    string UserId,
    string Role,
    DateTimeOffset JoinedAt);

public sealed record LaboratoryDetailsResponse(
    Guid Id,
    string Name,
    string Code,
    string? Description,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    IReadOnlyCollection<LaboratoryMemberResponse> Members);
