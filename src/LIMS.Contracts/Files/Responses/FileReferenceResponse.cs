namespace LIMS.Contracts.Files.Responses;

public sealed record FileReferenceResponse(
    Guid Id,
    Guid FileId,
    string ReferenceType,
    Guid ReferenceId,
    string LinkedByUserId,
    string? Purpose,
    DateTime LinkedAt);
