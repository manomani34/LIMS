using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Experiments.Commands;

public sealed record CancelExperimentCommand(
    Guid ExperimentId,
    string Reason) : ICommand<bool>;
