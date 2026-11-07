using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Experiments.DTOs;

namespace LIMS.Application.Experiments.Queries;

public sealed record GetExperimentByIdQuery(
    Guid ExperimentId) : IQuery<ExperimentDetailsDto?>;
