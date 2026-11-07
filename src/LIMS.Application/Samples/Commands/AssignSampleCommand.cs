using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Samples.Commands;

public sealed record AssignSampleCommand(
    Guid SampleId,
    string UserId) : ICommand<bool>;
