namespace LIMS.Contracts.Protocols;

public sealed record CreateProtocolRequest(
    Guid LaboratoryId,
    string ProtocolCode,
    string Name,
    string? Description,
    string Version,
    string CreatedByUserId);
