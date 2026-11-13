
using LIMS.Domain.Results;
using LIMS.Domain.Results.Enums;
using Xunit;

namespace LIMS.UnitTests.Results;

public sealed class ResultEntryTests
{
    private static readonly Guid LaboratoryId = Guid.NewGuid();
    private static readonly Guid ExperimentId = Guid.NewGuid();
    private static readonly Guid SampleId = Guid.NewGuid();

    private static AnalysisResult CreateResult()
    {
        return AnalysisResult.Create(
            LaboratoryId,
            ExperimentId,
            SampleId,
            null,
            "RES-001",
            "NMR Analysis",
            "Initial analysis",
            "researcher-01",
            DateTimeOffset.UtcNow);
    }

    [Fact]
    public void AddEntry_ShouldCreateNumericEntry()
    {
        var result = CreateResult();

        var entryId = result.AddEntry(
            "Purity",
            ResultValueType.Numeric,
            null,
            98.5m,
            "%",
            0.2m,
            "HPLC",
            "Purity measurement",
            DateTimeOffset.UtcNow);

        var entry = Assert.Single(result.Entries);

        Assert.Equal(entryId, entry.Id);
        Assert.Equal(result.Id, entry.ResultId);
        Assert.Equal("Purity", entry.ParameterName);
        Assert.Equal(ResultValueType.Numeric, entry.ValueType);
        Assert.Equal(98.5m, entry.NumericValue);
        Assert.Equal("%", entry.Unit);
        Assert.Equal(0.2m, entry.Uncertainty);
        Assert.Equal("HPLC", entry.Method);
        Assert.Equal("Purity measurement", entry.Notes);
    }

    [Fact]
    public void AddEntry_ShouldNormalizeParameterName()
    {
        var result = CreateResult();

        result.AddEntry(
            "  Purity  ",
            ResultValueType.Numeric,
            null,
            98.5m,
            "%",
            null,
            null,
            null,
            DateTimeOffset.UtcNow);

        Assert.Equal("Purity", Assert.Single(result.Entries).ParameterName);
    }

    [Fact]
    public void AddEntry_ShouldCreateTextEntry()
    {
        var result = CreateResult();

        result.AddEntry(
            "Appearance",
            ResultValueType.Text,
            "Yellow powder",
            null,
            null,
            null,
            null,
            null,
            DateTimeOffset.UtcNow);

        var entry = Assert.Single(result.Entries);

        Assert.Equal("Yellow powder", entry.Value);
        Assert.Null(entry.NumericValue);
    }

    [Fact]
    public void AddEntry_ShouldCreateBooleanEntry()
    {
        var result = CreateResult();

        result.AddEntry(
            "Passed",
            ResultValueType.Boolean,
            "true",
            null,
            null,
            null,
            null,
            null,
            DateTimeOffset.UtcNow);

        Assert.Equal("true", Assert.Single(result.Entries).Value);
    }

    [Fact]
    public void AddEntry_ShouldCreateDateEntry()
    {
        var result = CreateResult();

        result.AddEntry(
            "AnalysisDate",
            ResultValueType.Date,
            "2026-11-10",
            null,
            null,
            null,
            null,
            null,
            DateTimeOffset.UtcNow);

        Assert.Equal(
            "2026-11-10",
            Assert.Single(result.Entries).Value);
    }

    [Fact]
    public void AddEntry_ShouldRejectEmptyParameterName()
    {
        var result = CreateResult();

        Assert.Throws<AnalysisResultDomainException>(() =>
            result.AddEntry(
                " ",
                ResultValueType.Numeric,
                null,
                10m,
                null,
                null,
                null,
                null,
                DateTimeOffset.UtcNow));
    }

    [Fact]
    public void AddEntry_ShouldRejectDuplicateParameterNamesIgnoringCase()
    {
        var result = CreateResult();

        result.AddEntry(
            "Purity",
            ResultValueType.Numeric,
            null,
            98m,
            "%",
            null,
            null,
            null,
            DateTimeOffset.UtcNow);

        Assert.Throws<AnalysisResultDomainException>(() =>
            result.AddEntry(
                "purity",
                ResultValueType.Numeric,
                null,
                99m,
                "%",
                null,
                null,
                null,
                DateTimeOffset.UtcNow));
    }

    [Fact]
    public void AddEntry_ShouldRejectNegativeUncertainty()
    {
        var result = CreateResult();

        Assert.Throws<AnalysisResultDomainException>(() =>
            result.AddEntry(
                "Purity",
                ResultValueType.Numeric,
                null,
                98m,
                "%",
                -0.5m,
                null,
                null,
                DateTimeOffset.UtcNow));
    }

    [Fact]
    public void AddEntry_ShouldRejectTextEntryWithoutValue()
    {
        var result = CreateResult();

        Assert.Throws<AnalysisResultDomainException>(() =>
            result.AddEntry(
                "Appearance",
                ResultValueType.Text,
                " ",
                null,
                null,
                null,
                null,
                null,
                DateTimeOffset.UtcNow));
    }

    [Fact]
    public void AddEntry_ShouldRejectBooleanEntryWithInvalidValue()
    {
        var result = CreateResult();

        Assert.Throws<AnalysisResultDomainException>(() =>
            result.AddEntry(
                "Passed",
                ResultValueType.Boolean,
                "yes",
                null,
                null,
                null,
                null,
                null,
                DateTimeOffset.UtcNow));
    }

    [Fact]
    public void AddEntry_ShouldRejectInvalidDateValue()
    {
        var result = CreateResult();

        Assert.Throws<AnalysisResultDomainException>(() =>
            result.AddEntry(
                "AnalysisDate",
                ResultValueType.Date,
                "not-a-date",
                null,
                null,
                null,
                null,
                null,
                DateTimeOffset.UtcNow));
    }

    [Fact]
    public void UpdateEntry_ShouldUpdateExistingEntry()
    {
        var result = CreateResult();

        var entryId = result.AddEntry(
            "Purity",
            ResultValueType.Numeric,
            null,
            95m,
            "%",
            null,
            "HPLC",
            null,
            DateTimeOffset.UtcNow);

        result.UpdateEntry(
            entryId,
            "Purity",
            ResultValueType.Numeric,
            null,
            99.5m,
            "%",
            0.1m,
            "HPLC",
            "Updated measurement",
            DateTimeOffset.UtcNow.AddMinutes(1));

        var entry = Assert.Single(result.Entries);

        Assert.Equal(99.5m, entry.NumericValue);
        Assert.Equal(0.1m, entry.Uncertainty);
        Assert.Equal("Updated measurement", entry.Notes);
    }

    [Fact]
    public void UpdateEntry_ShouldRejectUnknownEntryId()
    {
        var result = CreateResult();

        Assert.Throws<AnalysisResultDomainException>(() =>
            result.UpdateEntry(
                Guid.NewGuid(),
                "Purity",
                ResultValueType.Numeric,
                null,
                99m,
                "%",
                null,
                null,
                null,
                DateTimeOffset.UtcNow));
    }

    [Fact]
    public void UpdateEntry_ShouldRejectDuplicateParameterName()
    {
        var result = CreateResult();

        var firstEntryId = result.AddEntry(
            "Purity",
            ResultValueType.Numeric,
            null,
            98m,
            "%",
            null,
            null,
            null,
            DateTimeOffset.UtcNow);

        result.AddEntry(
            "Moisture",
            ResultValueType.Numeric,
            null,
            2m,
            "%",
            null,
            null,
            null,
            DateTimeOffset.UtcNow);

        Assert.Throws<AnalysisResultDomainException>(() =>
            result.UpdateEntry(
                firstEntryId,
                "Moisture",
                ResultValueType.Numeric,
                null,
                99m,
                "%",
                null,
                null,
                null,
                DateTimeOffset.UtcNow));
    }

    [Fact]
    public void RemoveEntry_ShouldRemoveExistingEntry()
    {
        var result = CreateResult();

        var entryId = result.AddEntry(
            "Purity",
            ResultValueType.Numeric,
            null,
            98m,
            "%",
            null,
            null,
            null,
            DateTimeOffset.UtcNow);

        result.RemoveEntry(entryId, DateTimeOffset.UtcNow.AddMinutes(1));

        Assert.Empty(result.Entries);
    }

    [Fact]
    public void RemoveEntry_ShouldRejectUnknownEntryId()
    {
        var result = CreateResult();

        Assert.Throws<AnalysisResultDomainException>(() =>
            result.RemoveEntry(Guid.NewGuid(), DateTimeOffset.UtcNow));
    }
}
