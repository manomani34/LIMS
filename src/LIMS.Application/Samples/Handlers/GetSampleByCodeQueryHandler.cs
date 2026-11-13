using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Samples.DTOs;
using LIMS.Application.Samples.Interfaces;
using LIMS.Application.Samples.Queries;

namespace LIMS.Application.Samples.Handlers;

public sealed class GetSampleByCodeQueryHandler
    : IQueryHandler<GetSampleByCodeQuery, SampleDto?>
{
    private readonly ISampleRepository _sampleRepository;

    public GetSampleByCodeQueryHandler(ISampleRepository sampleRepository)
    {
        _sampleRepository = sampleRepository;
    }

    public async Task<SampleDto?> HandleAsync(
        GetSampleByCodeQuery query,
        CancellationToken cancellationToken = default)
    {
        var sample = await _sampleRepository.GetByCodeAsync(
            query.LaboratoryId,
            query.SampleCode,
            cancellationToken);

        if (sample is null)
            return null;

        return new SampleDto(
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
