using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Experiments.Commands;

public sealed record StartExperimentCommand(
    Guid ExperimentId) : ICommand<bool>;
