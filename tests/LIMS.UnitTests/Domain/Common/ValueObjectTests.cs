using LIMS.Domain.Common;

namespace LIMS.UnitTests.Domain.Common;

public sealed class ValueObjectTests
{
    [Fact]
    public void Equals_ShouldReturnTrueForSameComponents()
    {
        var first = new TestValueObject("Sample", 10);
        var second = new TestValueObject("Sample", 10);

        Assert.Equal(first, second);
    }

    [Fact]
    public void Equals_ShouldReturnFalseForDifferentComponents()
    {
        var first = new TestValueObject("Sample", 10);
        var second = new TestValueObject("Sample", 20);

        Assert.NotEqual(first, second);
    }

    [Fact]
    public void Equals_ShouldReturnFalseForDifferentTypes()
    {
        var first = new TestValueObject("Sample", 10);
        var second = new AnotherTestValueObject("Sample", 10);

        Assert.NotEqual(first, second);
    }

    [Fact]
    public void GetHashCode_ShouldReturnSameValueForEqualObjects()
    {
        var first = new TestValueObject("Sample", 10);
        var second = new TestValueObject("Sample", 10);

        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    private sealed class TestValueObject(
        string name,
        int value) : ValueObject
    {
        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return name;
            yield return value;
        }
    }

    private sealed class AnotherTestValueObject(
        string name,
        int value) : ValueObject
    {
        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return name;
            yield return value;
        }
    }
}
