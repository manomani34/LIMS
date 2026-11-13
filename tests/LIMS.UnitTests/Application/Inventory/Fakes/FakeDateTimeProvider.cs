
namespace LIMS.UnitTests.Application.Inventory.Fakes;

using LIMS.Application.Abstractions.Time;

public sealed class FakeDateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow { get; set; } =
        new(2026, 11, 13, 12, 0, 0, TimeSpan.Zero);
}
