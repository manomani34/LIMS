using LIMS.Domain.Results;
using LIMS.Domain.Results.Enums;
using Xunit;

namespace LIMS.UnitTests.Results;

public sealed class ResultEntryValidationTests
{
    private static readonly Guid LaboratoryId = Guid.NewGuid();
    private static readonly Guid ExperimentId = Guid.NewGuid();
    private static readonly Guid SampleId = Guid.NewGuid();

    private static readonly DateTime CreatedAt =
        new(2026, 11, 6, 10, 0, 0, DateTimeKind.Utc);

    private static AnalysisResult CreateResult()
    {
        return AnalysisResult.Create(
            LaboratoryId,
            ExperimentId,
            SampleId,
            null,
            "RES-VALIDATION-001",
            "Validation Test",
            null,
            "creator-001",
            CreatedAt);
    }

    [Fact]
    public void AddEntry_ShouldAcceptNumericValue()
    {
        var result = CreateResult();

        var entryId = result.AddEntry(
            "pH",
            ResultValueType.Numeric,
            null,
            7.2m,
            null,
            0.1m,
            "pH meter",
            null,
            CreatedAt.AddMinutes(1));

        Assert.NotEqual(Guid.Empty, entryId);
    }

    [Fact]
    public void AddEntry_ShouldRejectNumericValueWithoutMeasurement()
    {
        var result = CreateResult();

        Assert.Throws<AnalysisResultDomainException>(() =>
            result.AddEntry(
                "pH",
                ResultValueType.Numeric,
                null,
                null,
                null,
                null,
                null,
                null,
                CreatedAt.AddMinutes(1)));
    }

    [Fact]
    public void AddEntry_ShouldAcceptTextValue()
    {
        var result = CreateResult();

        var entryId = result.AddEntry(
            "Appearance",
            ResultValueType.Text,
            "Clear liquid",
            null,
            null,
            null,
            null,
            null,
            CreatedAt.AddMinutes(1));

        Assert.NotEqual(Guid.Empty, entryId);
    }

    [Fact]
    public void AddEntry_ShouldAcceptBooleanValue()
    {
        var result = CreateResult();

        var entryId = result.AddEntry(
            "PrecipitateDetected",
            ResultValueType.Boolean,
            "true",
            null,
            null,
            null,
            null,
            null,
            CreatedAt.AddMinutes(1));

        Assert.NotEqual(Guid.Empty, entryId);
    }

    [Fact]
    public void AddEntry_ShouldRejectInvalidBooleanValue()
    {
        var result = CreateResult();

        Assert.Throws<AnalysisResultDomainException>(() =>
            result.AddEntry(
                "PrecipitateDetected",
                ResultValueType.Boolean,
                "yes",
                null,
                null,
                null,
                null,
                null,
                CreatedAt.AddMinutes(1)));
    }

    [Fact]
    public void AddEntry_ShouldAcceptDateValue()
    {
        var result = CreateResult();

        var entryId = result.AddEntry(
            "MeasurementDate",
            ResultValueType.Date,
            "2026-11-06",
            null,
            null,
            null,
            null,
            null,
            CreatedAt.AddMinutes(1));

        Assert.NotEqual(Guid.Empty, entryId);
    }

    [Fact]
    public void AddEntry_ShouldRejectInvalidDateValue()
    {
        var result = CreateResult();

        Assert.Throws<AnalysisResultDomainException>(() =>
            result.AddEntry(
                "MeasurementDate",
                ResultValueType.Date,
                "invalid-date",
                null,
                null,
                null,
                null,
                null,
                CreatedAt.AddMinutes(1)));
    }

    [Fact]
    public void AddEntry_ShouldRejectDuplicateParameterIgnoringCase()
    {
        var result = CreateResult();

        result.AddEntry(
            "Temperature",
            ResultValueType.Numeric,
            null,
            25m,
            "C",
            null,
            null,
            null,
            CreatedAt.AddMinutes(1));

        Assert.Throws<AnalysisResultDomainException>(() =>
            result.AddEntry(
                "TEMPERATURE",
                ResultValueType.Numeric,
                null,
                26m,
                "C",
                null,
                null,
                null,
                CreatedAt.AddMinutes(2)));
    }

    [Fact]
    public void AddEntry_ShouldAllowParameterNameAgainAfterRemoval()
    {
        var result = CreateResult();

        var entryId = result.AddEntry(
            "Temperature",
            ResultValueType.Numeric,
            null,
            25m,
            "C",
            null,
            null,
            null,
            CreatedAt.AddMinutes(1));

        result.RemoveEntry(
            entryId,
            CreatedAt.AddMinutes(2));

        var newEntryId = result.AddEntry(
            "Temperature",
            ResultValueType.Numeric,
            null,
            26m,
            "C",
            null,
            null,
            null,
            CreatedAt.AddMinutes(3));

        Assert.NotEqual(entryId, newEntryId);
        Assert.Single(result.Entries);
    }
}
