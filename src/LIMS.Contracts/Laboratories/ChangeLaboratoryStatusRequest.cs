namespace LIMS.Contracts.Laboratories;

public sealed record ChangeLaboratoryStatusRequest(
    LaboratoryStatusContract Status);
