using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Experiments.Commands;

public sealed record UnassignExperimentCommand(
    Guid ExperimentId) : ICommand<bool>;
