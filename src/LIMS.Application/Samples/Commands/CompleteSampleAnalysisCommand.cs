using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Samples.Commands;

public sealed record CompleteSampleAnalysisCommand(
    Guid SampleId) : ICommand<bool>;
