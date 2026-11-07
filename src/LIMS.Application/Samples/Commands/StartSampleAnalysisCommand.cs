using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Samples.Commands;

public sealed record StartSampleAnalysisCommand(
    Guid SampleId) : ICommand<bool>;
