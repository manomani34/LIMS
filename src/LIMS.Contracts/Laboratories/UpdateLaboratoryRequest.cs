namespace LIMS.Contracts.Laboratories;

public sealed record UpdateLaboratoryRequest(
    string Name,
    string Code,
    string? Description);
