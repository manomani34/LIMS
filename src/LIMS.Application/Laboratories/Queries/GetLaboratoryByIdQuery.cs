using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Laboratories.DTOs;

namespace LIMS.Application.Laboratories.Queries;

public sealed record GetLaboratoryByIdQuery(
    Guid LaboratoryId) : IQuery<LaboratoryDetailsDto?>;
