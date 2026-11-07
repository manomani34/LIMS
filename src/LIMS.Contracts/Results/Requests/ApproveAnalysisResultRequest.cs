namespace LIMS.Contracts.Results.Requests;

public sealed record ApproveAnalysisResultRequest(
    string ReviewerUserId,
    string? Comments);
