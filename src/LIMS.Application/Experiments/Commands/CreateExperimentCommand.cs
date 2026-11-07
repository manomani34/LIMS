using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Experiments.Commands;

public sealed record CreateExperimentCommand(
    Guid LaboratoryId,
    Guid SampleId,
    string ExperimentCode,
    string Title,
    string? Description,
    string CreatedByUserId) : ICommand<Guid>;
