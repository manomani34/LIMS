namespace LIMS.Contracts.Requests.Requests;

public sealed record CancelSampleRequestRequest(
    string? Reason);
