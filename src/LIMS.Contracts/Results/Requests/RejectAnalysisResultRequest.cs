namespace LIMS.Contracts.Results.Requests;

public sealed record RejectAnalysisResultRequest(
    string ReviewerUserId,
    string Reason);
