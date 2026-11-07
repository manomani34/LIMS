using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Samples.Commands;

public sealed record StartSamplePreparationCommand(
    Guid SampleId) : ICommand<bool>;
