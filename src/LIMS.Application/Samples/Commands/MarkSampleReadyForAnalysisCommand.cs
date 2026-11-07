using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Samples.Commands;

public sealed record MarkSampleReadyForAnalysisCommand(
    Guid SampleId) : ICommand<bool>;
