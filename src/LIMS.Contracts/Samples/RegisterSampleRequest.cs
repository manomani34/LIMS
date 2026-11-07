namespace LIMS.Contracts.Samples;

public sealed record RegisterSampleRequest(
    Guid LaboratoryId,
    string SampleCode,
    string Name,
    string? Description,
    SampleTypeContract Type,
    SamplePriorityContract Priority,
    string? Source,
    string RegisteredByUserId);
