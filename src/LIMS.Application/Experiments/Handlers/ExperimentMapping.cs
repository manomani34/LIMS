using LIMS.Application.Experiments.DTOs;
using LIMS.Domain.Experiments;

namespace LIMS.Application.Experiments.Handlers;

internal static class ExperimentMapping
{
    public static ExperimentDto ToDto(this Experiment experiment)
    {
        return new ExperimentDto(
            experiment.Id,
            experiment.LaboratoryId,
            experiment.SampleId,
            experiment.ExperimentCode,
            experiment.Title,
            experiment.Description,
            experiment.Status,
            experiment.CreatedByUserId,
            experiment.AssignedToUserId,
            experiment.CreatedAt,
            experiment.UpdatedAt,
            experiment.StartedAt,
            experiment.CompletedAt,
            experiment.ArchivedAt);
    }

    public static ExperimentDetailsDto ToDetailsDto(this Experiment experiment)
    {
        return new ExperimentDetailsDto(
            experiment.Id,
            experiment.LaboratoryId,
            experiment.SampleId,
            experiment.ExperimentCode,
            experiment.Title,
            experiment.Description,
            experiment.Status,
            experiment.CreatedByUserId,
            experiment.AssignedToUserId,
            experiment.CreatedAt,
            experiment.UpdatedAt,
            experiment.StartedAt,
            experiment.CompletedAt,
            experiment.ArchivedAt);
    }
}
