using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Samples.Commands;

public sealed record UpdateSampleInformationCommand(
    Guid SampleId,
    string Name,
    string? Description,
    string? Source) : ICommand<bool>;
