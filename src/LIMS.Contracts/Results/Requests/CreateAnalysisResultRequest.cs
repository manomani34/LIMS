namespace LIMS.Contracts.Results.Requests;

public sealed record CreateAnalysisResultRequest(
    Guid LaboratoryId,
    Guid ExperimentId,
    Guid SampleId,
    Guid? InstrumentId,
    string ResultCode,
    string Title,
    string? Description,
    string CreatedByUserId);
