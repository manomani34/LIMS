namespace LIMS.Contracts.Laboratories;

public sealed record CreateLaboratoryRequest(
    string Name,
    string Code,
    string? Description);
