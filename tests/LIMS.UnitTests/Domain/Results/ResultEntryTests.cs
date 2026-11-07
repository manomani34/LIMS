using LIMS.Domain.Results.Entities;
using LIMS.Domain.Results.Enums;
using LIMS.Domain.Results.Exceptions;
using Xunit;

namespace LIMS.UnitTests.Results;

public sealed class ResultEntryTests
{
    private static readonly Guid ResultId =
        Guid.NewGuid();

    private static readonly DateTime CreatedAt =
        new(2026, 11, 6, 10, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Constructor_ShouldCreateNumericEntry()
    {
        var entry = new ResultEntry(
            ResultId,
            "Chemical Shift",
            ResultValueType.Numeric,
            null,
            7.25m,
            "ppm",
            null,
            "NMR",
            null);

        Assert.NotEqual(Guid.Empty, entry.Id);
        Assert.Equal(ResultId, entry.ResultId);
        Assert.Equal("Chemical Shift", entry.ParameterName);
        Assert.Equal(ResultValueType.Numeric, entry.ValueType);
        Assert.Equal(7.25m, entry.NumericValue);
    }

    [Fact]
    public void Constructor_ShouldRejectEmptyResultId()
    {
        Assert.Throws<AnalysisResultDomainException>(() =>
            new ResultEntry(
                Guid.Empty,
                "Chemical Shift",
                ResultValueType.Numeric,
                null,
                7.25m,
                "ppm",
                null,
                null,
                null));
    }

    [Fact]
    public void Constructor_ShouldRejectNumericEntryWithoutNumericValue()
    {
        Assert.Throws<AnalysisResultDomainException>(() =>
            new ResultEntry(
                ResultId,
                "Chemical Shift",
                ResultValueType.Numeric,
                null,
                null,
                "ppm",
                null,
                null,
                null));
    }

    [Fact]
    public void Constructor_ShouldRejectNegativeUncertainty()
    {
        Assert.Throws<AnalysisResultDomainException>(() =>
            new ResultEntry(
                ResultId,
                "Chemical Shift",
                ResultValueType.Numeric,
                null,
                7.25m,
                "ppm",
                -0.1m,
                null,
                null));
    }

    [Fact]
    public void Constructor_ShouldCreateTextEntry()
    {
        var entry = new ResultEntry(
            ResultId,
            "Observation",
            ResultValueType.Text,
            "Yellow solution",
            null,
            null,
            null,
            null,
            null);

        Assert.Equal(ResultValueType.Text, entry.ValueType);
        Assert.Equal("Yellow solution", entry.Value);
    }

    [Fact]
    public void Constructor_ShouldRejectTextEntryWithoutValue()
    {
        Assert.Throws<AnalysisResultDomainException>(() =>
            new ResultEntry(
                ResultId,
                "Observation",
                ResultValueType.Text,
                null,
                null,
                null,
                null,
                null,
                null));
    }

    [Theory]
    [InlineData("true")]
    [InlineData("false")]
    [InlineData("TRUE")]
    [InlineData("FALSE")]
    public void Constructor_ShouldAcceptBooleanValues(string value)
    {
        var entry = new ResultEntry(
            ResultId,
            "Detected",
            ResultValueType.Boolean,
            value,
            null,
            null,
            null,
            null,
            null);

        Assert.Equal(value, entry.Value);
    }

    [Theory]
    [InlineData("yes")]
    [InlineData("no")]
    [InlineData("1")]
    [InlineData("0")]
    public void Constructor_ShouldRejectInvalidBooleanValues(string value)
    {
        Assert.Throws<AnalysisResultDomainException>(() =>
            new ResultEntry(
                ResultId,
                "Detected",
                ResultValueType.Boolean,
                value,
                null,
                null,
                null,
                null,
                null));
    }

    [Theory]
    [InlineData("2026-11-06")]
    [InlineData("2025-01-15")]
    public void Constructor_ShouldAcceptValidDateValues(string value)
    {
        var entry = new ResultEntry(
            ResultId,
            "Analysis Date",
            ResultValueType.Date,
            value,
            null,
            null,
            null,
            null,
            null);

        Assert.Equal(value, entry.Value);
    }

    [Fact]
    public void Constructor_ShouldRejectInvalidDateValue()
    {
        Assert.Throws<AnalysisResultDomainException>(() =>
            new ResultEntry(
                ResultId,
                "Analysis Date",
                ResultValueType.Date,
                "not-a-date",
                null,
                null,
                null,
                null,
                null));
    }
}
