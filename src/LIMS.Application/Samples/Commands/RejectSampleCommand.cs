using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Samples.Commands;

public sealed record RejectSampleCommand(
    Guid SampleId,
    string Reason) : ICommand<bool>;
