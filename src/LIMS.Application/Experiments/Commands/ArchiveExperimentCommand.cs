using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Experiments.Commands;

public sealed record ArchiveExperimentCommand(
    Guid ExperimentId) : ICommand<bool>;
