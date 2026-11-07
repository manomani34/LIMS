using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Samples.Commands;

public sealed record ReceiveSampleCommand(
    Guid SampleId) : ICommand<bool>;
