namespace LIMS.Contracts.Experiments.Requests;

public sealed record CancelExperimentRequest(
    string Reason);
