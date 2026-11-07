using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Samples.Commands;

public sealed record UnassignSampleCommand(
    Guid SampleId) : ICommand<bool>;
