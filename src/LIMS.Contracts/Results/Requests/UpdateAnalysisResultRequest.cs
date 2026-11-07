namespace LIMS.Contracts.Results.Requests;

public sealed record UpdateAnalysisResultRequest(
    string Title,
    string? Description,
    Guid? InstrumentId);
