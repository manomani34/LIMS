using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Experiments.Commands;

public sealed record CompleteExperimentCommand(
    Guid ExperimentId) : ICommand<bool>;
