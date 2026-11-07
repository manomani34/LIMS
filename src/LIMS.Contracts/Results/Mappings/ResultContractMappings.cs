using LIMS.Contracts.Results.Enums;
using LIMS.Contracts.Results.Responses;
using LIMS.Domain.Results.Enums;

namespace LIMS.Contracts.Results.Mappings;

public static class ResultContractMappings
{
    public static ResultStatus ToDomain(this ResultStatusContract status)
    {
        if (!Enum.IsDefined(status))
            throw new ArgumentOutOfRangeException(nameof(status));

        return (ResultStatus)(int)status;
    }

    public static ResultStatusContract ToContract(this ResultStatus status)
    {
        if (!Enum.IsDefined(status))
            throw new ArgumentOutOfRangeException(nameof(status));

        return (ResultStatusContract)(int)status;
    }

    public static ResultValueType ToDomain(this ResultValueTypeContract valueType)
    {
        if (!Enum.IsDefined(valueType))
            throw new ArgumentOutOfRangeException(nameof(valueType));

        return (ResultValueType)(int)valueType;
    }

    public static ResultValueTypeContract ToContract(this ResultValueType valueType)
    {
        if (!Enum.IsDefined(valueType))
            throw new ArgumentOutOfRangeException(nameof(valueType));

        return (ResultValueTypeContract)(int)valueType;
    }

    public static ResultEntryResponse ToResponse(
        this LIMS.Application.Results.DTOs.ResultEntryDto entry)
    {
        return new ResultEntryResponse(
            entry.Id,
            entry.ResultId,
            entry.ParameterName,
            entry.ValueType.ToContract(),
            entry.Value,
            entry.NumericValue,
            entry.Unit,
            entry.Uncertainty,
            entry.Method,
            entry.Notes);
    }

    public static AnalysisResultResponse ToResponse(
        this LIMS.Application.Results.DTOs.AnalysisResultDto result)
    {
        return new AnalysisResultResponse(
            result.Id,
            result.LaboratoryId,
            result.ExperimentId,
            result.SampleId,
            result.InstrumentId,
            result.ResultCode,
            result.Title,
            result.Description,
            result.Status.ToContract(),
            result.CreatedByUserId,
            result.SubmittedByUserId,
            result.ReviewedByUserId,
            result.ReviewComments,
            result.RejectionReason,
            result.ArchivedByUserId,
            result.CreatedAt,
            result.UpdatedAt,
            result.SubmittedAt,
            result.ReviewedAt,
            result.ArchivedAt);
    }

    public static AnalysisResultDetailsResponse ToDetailsResponse(
        this LIMS.Application.Results.DTOs.AnalysisResultDetailsDto result)
    {
        return new AnalysisResultDetailsResponse(
            result.ToResponse(),
            result.Entries.Select(x => x.ToResponse()).ToArray());
    }
}
