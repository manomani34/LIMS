using LIMS.Application.Results.DTOs;
using LIMS.Domain.Results;

namespace LIMS.Application.Results.Mapping;

public static class AnalysisResultMapping
{
    public static AnalysisResultDto ToDto(AnalysisResult result)
    {
        return new AnalysisResultDto(
            result.Id,
            result.LaboratoryId,
            result.ExperimentId,
            result.SampleId,
            result.InstrumentId,
            result.ResultCode,
            result.Title,
            result.Description,
            result.Status,
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

    public static AnalysisResultDetailsDto ToDetailsDto(
        AnalysisResult result)
    {
        return new AnalysisResultDetailsDto(
            result.Id,
            result.LaboratoryId,
            result.ExperimentId,
            result.SampleId,
            result.InstrumentId,
            result.ResultCode,
            result.Title,
            result.Description,
            result.Status,
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
            result.ArchivedAt,
            result.Entries.Select(entry => new ResultEntryDto(
                entry.Id,
                entry.ResultId,
                entry.ParameterName,
                entry.ValueType,
                entry.Value,
                entry.NumericValue,
                entry.Unit,
                entry.Uncertainty,
                entry.Method,
                entry.Notes)).ToArray());
    }
}
