using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Experiments.DTOs;

namespace LIMS.Application.Experiments.Queries;

public sealed record GetExperimentByCodeQuery(
    Guid LaboratoryId,
    string ExperimentCode) : IQuery<ExperimentDto?>;
