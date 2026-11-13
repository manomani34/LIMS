using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Samples.DTOs;
using LIMS.Application.Samples.Interfaces;
using LIMS.Application.Samples.Queries;

namespace LIMS.Application.Samples.Handlers;

public sealed class GetSampleByIdQueryHandler
    : IQueryHandler<GetSampleByIdQuery, SampleDetailsDto?>
{
    private readonly ISampleRepository _sampleRepository;

    public GetSampleByIdQueryHandler(ISampleRepository sampleRepository)
    {
        _sampleRepository = sampleRepository;
    }

    public async Task<SampleDetailsDto?> HandleAsync(
        GetSampleByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var sample = await _sampleRepository.GetWithDetailsAsync(
            query.SampleId,
            cancellationToken);

        if (sample is null)
            return null;

        return new SampleDetailsDto(
            sample.Id,
            sample.LaboratoryId,
            sample.SampleCode,
            sample.Name,
            sample.Description,
            sample.RejectionReason,
            sample.Type,
            sample.Status,
            sample.Priority,
            sample.Source,
            sample.AssignedToUserId,
            sample.RegisteredByUserId,
            sample.RegisteredAt,
            sample.ReceivedAt,
            sample.UpdatedAt,
            sample.ArchivedAt);
    }
}
