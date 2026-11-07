namespace LIMS.Contracts.Protocols;

public sealed record UpdateProtocolRequest(
    string Name,
    string? Description,
    string Version);
