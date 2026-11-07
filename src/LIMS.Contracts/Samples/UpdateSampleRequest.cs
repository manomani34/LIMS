namespace LIMS.Contracts.Samples;

public sealed record UpdateSampleRequest(
    string Name,
    string? Description,
    string? Source);
