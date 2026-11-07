using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Experiments.Commands;

public sealed record AssignExperimentCommand(
    Guid ExperimentId,
    string UserId) : ICommand<bool>;
