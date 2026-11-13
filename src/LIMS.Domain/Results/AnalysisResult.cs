using LIMS.Domain.Common;
using LIMS.Domain.Results.Enums;
using LIMS.Domain.Results.Events;

namespace LIMS.Domain.Results;

public sealed class AnalysisResult : AggregateRoot<Guid>
{
    private readonly List<ResultEntry> _entries = [];

    public Guid LaboratoryId { get; private set; }
    public Guid ExperimentId { get; private set; }
    public Guid SampleId { get; private set; }
    public Guid? InstrumentId { get; private set; }

    public string ResultCode { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    public ResultStatus Status { get; private set; }

    public string CreatedByUserId { get; private set; } = string.Empty;
    public string? SubmittedByUserId { get; private set; }
    public string? ReviewedByUserId { get; private set; }
    public string? ReviewComments { get; private set; }
    public string? RejectionReason { get; private set; }
    public string? ArchivedByUserId { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public DateTimeOffset? SubmittedAt { get; private set; }
    public DateTimeOffset? ReviewedAt { get; private set; }
    public DateTimeOffset? ArchivedAt { get; private set; }

    public IReadOnlyCollection<ResultEntry> Entries => _entries.AsReadOnly();

    private AnalysisResult()
    {
    }

    private AnalysisResult(
        Guid id,
        Guid laboratoryId,
        Guid experimentId,
        Guid sampleId,
        Guid? instrumentId,
        string resultCode,
        string title,
        string? description,
        string createdByUserId,
        DateTimeOffset createdAt) : base(id)
    {
        LaboratoryId = laboratoryId;
        ExperimentId = experimentId;
        SampleId = sampleId;
        InstrumentId = instrumentId;
        ResultCode = resultCode;
        Title = title;
        Description = description;
        CreatedByUserId = createdByUserId;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
        Status = ResultStatus.Draft;
    }

    public static AnalysisResult Create(
        Guid laboratoryId,
        Guid experimentId,
        Guid sampleId,
        Guid? instrumentId,
        string resultCode,
        string title,
        string? description,
        string createdByUserId,
        DateTimeOffset createdAt)
    {
        if (laboratoryId == Guid.Empty)
            throw new AnalysisResultDomainException("LaboratoryId is required.");

        if (experimentId == Guid.Empty)
            throw new AnalysisResultDomainException("ExperimentId is required.");

        if (sampleId == Guid.Empty)
            throw new AnalysisResultDomainException("SampleId is required.");

        if (instrumentId == Guid.Empty)
            throw new AnalysisResultDomainException("InstrumentId cannot be empty.");

        resultCode = NormalizeRequired(resultCode, 50, nameof(resultCode));
        if (!IsValidCode(resultCode))
            throw new AnalysisResultDomainException(
                "Result code contains invalid characters.");

        title = NormalizeRequired(title, 200, nameof(title));
        description = NormalizeOptional(description, 4000);
        createdByUserId = NormalizeRequired(
            createdByUserId,
            150,
            nameof(createdByUserId));

        var result = new AnalysisResult(
            Guid.NewGuid(),
            laboratoryId,
            experimentId,
            sampleId,
            instrumentId,
            resultCode,
            title,
            description,
            createdByUserId,
            createdAt);

        result.AddDomainEvent(new AnalysisResultCreatedDomainEvent(
            result.Id,
            laboratoryId,
            experimentId,
            sampleId,
            resultCode,
            createdAt));

        return result;
    }

    public void UpdateInformation(
    string title,
    string? description,
    Guid? instrumentId,
    DateTimeOffset updatedAt)
    {
        EnsureEditable();

        if (instrumentId == Guid.Empty)
            throw new AnalysisResultDomainException(
                "InstrumentId cannot be empty.");

        Title = NormalizeRequired(title, 200, nameof(title));
        Description = NormalizeOptional(description, 4000);
        InstrumentId = instrumentId;
        UpdatedAt = updatedAt;
    }

    public Guid AddEntry(
        string parameterName,
        ResultValueType valueType,
        string? value,
        decimal? numericValue,
        string? unit,
        decimal? uncertainty,
        string? method,
        string? notes,
        DateTimeOffset occurredOn)
    {
        EnsureEditable();

        parameterName = NormalizeRequired(
            parameterName,
            150,
            nameof(parameterName));

        EnsureUniqueParameter(parameterName);

        var entry = new ResultEntry(
            Guid.NewGuid(),
            Id,
            parameterName,
            valueType,
            value,
            numericValue,
            unit,
            uncertainty,
            method,
            notes);

        _entries.Add(entry);
        UpdatedAt = occurredOn;

        AddDomainEvent(new ResultEntryAddedDomainEvent(
            Id,
            entry.Id,
            entry.ParameterName,
            occurredOn));

        return entry.Id;
    }

    public void UpdateEntry(
        Guid entryId,
        string parameterName,
        ResultValueType valueType,
        string? value,
        decimal? numericValue,
        string? unit,
        decimal? uncertainty,
        string? method,
        string? notes,
        DateTimeOffset occurredOn)
    {
        EnsureEditable();

        var entry = _entries.FirstOrDefault(x => x.Id == entryId);

        if (entry is null)
            throw new AnalysisResultDomainException("Result entry was not found.");

        parameterName = NormalizeRequired(
            parameterName,
            150,
            nameof(parameterName));

        EnsureUniqueParameter(parameterName, entryId);

        entry.Update(
            parameterName,
            valueType,
            value,
            numericValue,
            unit,
            uncertainty,
            method,
            notes);

        UpdatedAt = occurredOn;

        AddDomainEvent(new ResultEntryUpdatedDomainEvent(
            Id,
            entry.Id,
            occurredOn));
    }

    public void RemoveEntry(Guid entryId, DateTimeOffset occurredOn)
    {
        EnsureEditable();

        var entry = _entries.FirstOrDefault(x => x.Id == entryId);

        if (entry is null)
            throw new AnalysisResultDomainException("Result entry was not found.");

        _entries.Remove(entry);
        UpdatedAt = occurredOn;

        AddDomainEvent(new ResultEntryRemovedDomainEvent(
            Id,
            entry.Id,
            entry.ParameterName,
            occurredOn));
    }

    public void Submit(string submittedByUserId, DateTimeOffset submittedAt)
    {
        EnsureStatus(ResultStatus.Draft);

        if (_entries.Count == 0)
            throw new AnalysisResultDomainException(
                "At least one result entry is required before submission.");

        submittedByUserId = NormalizeRequired(
            submittedByUserId,
            150,
            nameof(submittedByUserId));

        SubmittedByUserId = submittedByUserId;
        SubmittedAt = submittedAt;
        Status = ResultStatus.Submitted;
        UpdatedAt = submittedAt;

        AddDomainEvent(new AnalysisResultSubmittedDomainEvent(
            Id,
            submittedByUserId,
            submittedAt,
            submittedAt));
    }

    public void StartReview(
        string reviewerUserId,
        DateTimeOffset startedAt)
    {
        EnsureStatus(ResultStatus.Submitted);

        reviewerUserId = NormalizeRequired(
            reviewerUserId,
            150,
            nameof(reviewerUserId));

        if (string.Equals(
            reviewerUserId,
            SubmittedByUserId,
            StringComparison.OrdinalIgnoreCase))
        {
            throw new AnalysisResultDomainException(
                "The submitter cannot review their own result.");
        }

        ReviewedByUserId = reviewerUserId;
        Status = ResultStatus.UnderReview;
        UpdatedAt = startedAt;

        AddDomainEvent(new AnalysisResultReviewStartedDomainEvent(
            Id,
            reviewerUserId,
            startedAt,
            startedAt));
    }

    public void Approve(
        string reviewerUserId,
        string? comments,
        DateTimeOffset approvedAt)
    {
        EnsureStatus(ResultStatus.UnderReview);
        EnsureAssignedReviewer(reviewerUserId);

        ReviewComments = NormalizeOptional(comments, 2000);
        RejectionReason = null;
        ReviewedAt = approvedAt;
        Status = ResultStatus.Approved;
        UpdatedAt = approvedAt;

        AddDomainEvent(new AnalysisResultApprovedDomainEvent(
            Id,
            reviewerUserId,
            ReviewComments,
            approvedAt,
            approvedAt));
    }

    public void Reject(
        string reviewerUserId,
        string reason,
        DateTimeOffset rejectedAt)
    {
        EnsureStatus(ResultStatus.UnderReview);
        EnsureAssignedReviewer(reviewerUserId);

        RejectionReason = NormalizeRequired(
            reason,
            2000,
            nameof(reason));

        ReviewComments = null;
        ReviewedAt = rejectedAt;
        Status = ResultStatus.Rejected;
        UpdatedAt = rejectedAt;

        AddDomainEvent(new AnalysisResultRejectedDomainEvent(
            Id,
            reviewerUserId,
            RejectionReason,
            rejectedAt,
            rejectedAt));
    }

    public void ReturnToDraft(
        string requestedByUserId,
        DateTimeOffset returnedAt)
    {
        EnsureStatus(ResultStatus.Rejected);

        requestedByUserId = NormalizeRequired(
            requestedByUserId,
            150,
            nameof(requestedByUserId));

        if (!string.Equals(
            requestedByUserId,
            CreatedByUserId,
            StringComparison.OrdinalIgnoreCase))
        {
            throw new AnalysisResultDomainException(
                "Only the result creator can return a rejected result to draft.");
        }

        Status = ResultStatus.Draft;
        SubmittedByUserId = null;
        SubmittedAt = null;
        ReviewedByUserId = null;
        ReviewedAt = null;
        ReviewComments = null;
        RejectionReason = null;
        UpdatedAt = returnedAt;
    }

    public void Archive(
        string archivedByUserId,
        DateTimeOffset archivedAt)
    {
        if (Status is not ResultStatus.Approved and not ResultStatus.Rejected)
        {
            throw new AnalysisResultDomainException(
                "Only approved or rejected results can be archived.");
        }

        archivedByUserId = NormalizeRequired(
            archivedByUserId,
            150,
            nameof(archivedByUserId));

        ArchivedByUserId = archivedByUserId;
        ArchivedAt = archivedAt;
        Status = ResultStatus.Archived;
        UpdatedAt = archivedAt;

        AddDomainEvent(new AnalysisResultArchivedDomainEvent(
            Id,
            archivedByUserId,
            archivedAt,
            archivedAt));
    }

    private void EnsureEditable()
    {
        EnsureStatus(ResultStatus.Draft);
    }

    private void EnsureStatus(ResultStatus expectedStatus)
    {
        if (Status != expectedStatus)
        {
            throw new AnalysisResultDomainException(
                $"Operation is not allowed while result status is {Status}.");
        }
    }

    private void EnsureAssignedReviewer(string reviewerUserId)
    {
        reviewerUserId = NormalizeRequired(
            reviewerUserId,
            150,
            nameof(reviewerUserId));

        if (!string.Equals(
            reviewerUserId,
            ReviewedByUserId,
            StringComparison.OrdinalIgnoreCase))
        {
            throw new AnalysisResultDomainException(
                "Only the assigned reviewer can complete the review.");
        }
    }

    private void EnsureUniqueParameter(
        string parameterName,
        Guid? excludedEntryId = null)
    {
        var exists = _entries.Any(x =>
            x.Id != excludedEntryId &&
            string.Equals(
                x.ParameterName,
                parameterName,
                StringComparison.OrdinalIgnoreCase));

        if (exists)
        {
            throw new AnalysisResultDomainException(
                "A result entry with this parameter name already exists.");
        }
    }

    private static string NormalizeRequired(
        string? value,
        int maxLength,
        string parameterName)
    {
        var normalized = value?.Trim();

        if (string.IsNullOrWhiteSpace(normalized))
            throw new AnalysisResultDomainException(
                $"{parameterName} is required.");

        if (normalized.Length > maxLength)
            throw new AnalysisResultDomainException(
                $"{parameterName} cannot exceed {maxLength} characters.");

        return normalized;
    }

    private static string? NormalizeOptional(
        string? value,
        int maxLength)
    {
        var normalized = value?.Trim();

        if (string.IsNullOrWhiteSpace(normalized))
            return null;

        if (normalized.Length > maxLength)
            throw new AnalysisResultDomainException(
                $"Value cannot exceed {maxLength} characters.");

        return normalized;
    }

    private static bool IsValidCode(string value)
    {
        return value.All(c =>
            c is >= 'A' and <= 'Z' ||
            c is >= '0' and <= '9' ||
            c is '-' or '_');
    }
}

