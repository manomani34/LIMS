namespace LIMS.Contracts.Results.Responses;

public sealed record AnalysisResultDetailsResponse(
    AnalysisResultResponse Result,
    IReadOnlyCollection<ResultEntryResponse> Entries);
