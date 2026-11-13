using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Requests.Commands;

public sealed record FulfillSampleRequestCommand(
    Guid RequestId,
    Guid SampleId) : ICommand<bool>;
