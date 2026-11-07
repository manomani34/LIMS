using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Samples.Commands;

public sealed record ArchiveSampleCommand(
    Guid SampleId) : ICommand<bool>;
