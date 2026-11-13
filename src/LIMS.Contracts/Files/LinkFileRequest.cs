namespace LIMS.Contracts.Files;

public sealed record LinkFileRequest(
    FileReferenceTypeContract ReferenceType,
    Guid ReferenceId,
    string LinkedByUserId,
    string? Purpose);
