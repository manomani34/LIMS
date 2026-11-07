namespace LIMS.Contracts.Requests.Requests;

public sealed record ApproveSampleRequestRequest(
    Guid LaboratoryId,
    string? Comments);
