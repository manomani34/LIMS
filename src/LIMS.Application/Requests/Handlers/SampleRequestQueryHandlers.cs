using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Requests.DTOs;
using LIMS.Application.Requests.Mapping;
using LIMS.Application.Requests.Queries;
using LIMS.Application.Requests.Repositories;

namespace LIMS.Application.Requests.Handlers;

public sealed class GetSampleRequestByIdQueryHandler
    : IQueryHandler<GetSampleRequestByIdQuery, SampleRequestDto?>
{
    private readonly ISampleRequestRepository _repository;

    public GetSampleRequestByIdQueryHandler(
        ISampleRequestRepository repository)
    {
        _repository = repository;
    }

    public async Task<SampleRequestDto?> HandleAsync(
        GetSampleRequestByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var request = await _repository.GetWithDetailsAsync(
            query.RequestId,
            cancellationToken);

        return request?.ToDto();
    }
}

public sealed class GetSampleRequestByCodeQueryHandler
    : IQueryHandler<GetSampleRequestByCodeQuery, SampleRequestDto?>
{
    private readonly ISampleRequestRepository _repository;

    public GetSampleRequestByCodeQueryHandler(
        ISampleRequestRepository repository)
    {
        _repository = repository;
    }

    public async Task<SampleRequestDto?> HandleAsync(
        GetSampleRequestByCodeQuery query,
        CancellationToken cancellationToken = default)
    {
        var request = await _repository.GetByCodeAsync(
            query.RequestCode,
            cancellationToken);

        return request?.ToDto();
    }
}

public sealed class GetSampleRequestsByRequesterQueryHandler
    : IQueryHandler<
        GetSampleRequestsByRequesterQuery,
        IReadOnlyCollection<SampleRequestDto>>
{
    private readonly ISampleRequestRepository _repository;

    public GetSampleRequestsByRequesterQueryHandler(
        ISampleRequestRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<SampleRequestDto>> HandleAsync(
        GetSampleRequestsByRequesterQuery query,
        CancellationToken cancellationToken = default)
    {
        var requests = await _repository.GetByRequesterAsync(
            query.RequestedByUserId,
            cancellationToken);

        return requests.Select(x => x.ToDto()).ToArray();
    }
}

public sealed class GetSampleRequestsByLaboratoryQueryHandler
    : IQueryHandler<
        GetSampleRequestsByLaboratoryQuery,
        IReadOnlyCollection<SampleRequestDto>>
{
    private readonly ISampleRequestRepository _repository;

    public GetSampleRequestsByLaboratoryQueryHandler(
        ISampleRequestRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<SampleRequestDto>> HandleAsync(
        GetSampleRequestsByLaboratoryQuery query,
        CancellationToken cancellationToken = default)
    {
        var requests = await _repository.GetByLaboratoryIdAsync(
            query.LaboratoryId,
            cancellationToken);

        return requests.Select(x => x.ToDto()).ToArray();
    }
}

public sealed class GetSampleRequestsByStatusQueryHandler
    : IQueryHandler<
        GetSampleRequestsByStatusQuery,
        IReadOnlyCollection<SampleRequestDto>>
{
    private readonly ISampleRequestRepository _repository;

    public GetSampleRequestsByStatusQueryHandler(
        ISampleRequestRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<SampleRequestDto>> HandleAsync(
        GetSampleRequestsByStatusQuery query,
        CancellationToken cancellationToken = default)
    {
        var requests = await _repository.GetByStatusAsync(
            query.Status,
            cancellationToken);

        return requests.Select(x => x.ToDto()).ToArray();
    }
}

public sealed class GetPendingSampleRequestsQueryHandler
    : IQueryHandler<
        GetPendingSampleRequestsQuery,
        IReadOnlyCollection<SampleRequestDto>>
{
    private readonly ISampleRequestRepository _repository;

    public GetPendingSampleRequestsQueryHandler(
        ISampleRequestRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<SampleRequestDto>> HandleAsync(
        GetPendingSampleRequestsQuery query,
        CancellationToken cancellationToken = default)
    {
        var requests = await _repository.GetPendingReviewAsync(
            cancellationToken);

        return requests.Select(x => x.ToDto()).ToArray();
    }
}
