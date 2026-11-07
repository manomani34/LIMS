using LIMS.Contracts.Requests.Enums;
using LIMS.Contracts.Samples;

namespace LIMS.Contracts.Requests.Requests;

public sealed record CreateSampleRequestRequest(
    string RequestCode,
    string Title,
    string? Description,
    string SampleName,
    string? SampleDescription,
    SampleTypeContract RequestedSampleType,
    string? SampleSource,
    SampleRequestPriorityContract Priority);
