using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Experiments.Commands;

public sealed record UpdateExperimentInformationCommand(
    Guid ExperimentId,
    string Title,
    string? Description) : ICommand<bool>;
