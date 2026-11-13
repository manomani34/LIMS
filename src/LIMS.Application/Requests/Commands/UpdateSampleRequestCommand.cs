using LIMS.Application.Abstractions.Messaging;
using LIMS.Domain.Requests.Enums;
using LIMS.Domain.Samples.Enums;

namespace LIMS.Application.Requests.Commands;

public sealed record UpdateSampleRequestCommand(
    Guid RequestId,
    string Title,
    string? Description,
    string SampleName,
    string? SampleDescription,
    SampleType RequestedSampleType,
    string? SampleSource,
    SampleRequestPriority Priority) : ICommand<bool>;
