namespace LIMS.Contracts.Experiments.Requests;

public sealed record UpdateExperimentRequest(
    string Title,
    string? Description);
