namespace LIMS.Contracts.Experiments.Requests;

public sealed record CreateExperimentRequest(
    Guid LaboratoryId,
    Guid SampleId,
    string ExperimentCode,
    string Title,
    string? Description,
    string CreatedByUserId);
