using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Samples.DTOs;

namespace LIMS.Application.Samples.Queries;

public sealed record GetSampleByIdQuery(
    Guid SampleId) : IQuery<SampleDetailsDto?>;
