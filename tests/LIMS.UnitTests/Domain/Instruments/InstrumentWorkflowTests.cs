using LIMS.Domain.Instruments;
using LIMS.Domain.Instruments.Enums;

namespace LIMS.UnitTests.Domain.Instruments;

public sealed class InstrumentWorkflowTests
{
    private static readonly Guid LaboratoryId =
        Guid.Parse("11111111-1111-1111-1111-111111111111");

    private static Instrument CreateInstrument()
    {
        return Instrument.Register(
            LaboratoryId,
            "HPLC-001",
            "HPLC System",
            InstrumentType.Chromatograph,
            DateTimeOffset.UtcNow,
            "Agilent",
            "1260 Infinity",
            "HPLC-SN-001",
            "Analytical Laboratory",
            null);
    }

    [Fact]
    public void CompleteLifecycle_ShouldRetireInstrument()
    {
        var instrument = CreateInstrument();
        var now = DateTimeOffset.UtcNow;

        instrument.MarkInUse(now.AddMinutes(1));
        instrument.MarkAvailable(now.AddMinutes(2));

        instrument.ScheduleMaintenance(
            MaintenanceType.Preventive,
            "Scheduled maintenance",
            now.AddDays(1),
            "technician-001",
            now.AddMinutes(3));

        var maintenanceId = instrument.MaintenanceRecords.Single().Id;

        instrument.CompleteMaintenance(
            maintenanceId,
            "technician-001",
            "Maintenance completed",
            now.AddDays(2));

        instrument.Retire(now.AddDays(3));

        Assert.Equal(InstrumentStatus.Retired, instrument.Status);
        Assert.NotNull(instrument.RetiredAt);
        Assert.Single(instrument.MaintenanceRecords);
        Assert.NotNull(instrument.MaintenanceRecords.Single().CompletedAt);
    }

    [Fact]
    public void Retire_ShouldThrow_WhenActiveMaintenanceExists()
    {
        var instrument = CreateInstrument();
        var now = DateTimeOffset.UtcNow;

        instrument.ScheduleMaintenance(
            MaintenanceType.Corrective,
            "Repair required",
            now.AddDays(1),
            "technician-001",
            now);

        Assert.Throws<InstrumentDomainException>(() =>
            instrument.Retire(now.AddDays(2)));
    }

    [Fact]
    public void RestoreService_ShouldThrow_WhenActiveMaintenanceExists()
    {
        var instrument = CreateInstrument();
        var now = DateTimeOffset.UtcNow;

        instrument.MarkOutOfService(
            "Electrical fault",
            now);

        instrument.ScheduleMaintenance(
            MaintenanceType.Corrective,
            "Electrical repair",
            now.AddDays(1),
            "technician-001",
            now.AddMinutes(1));

        Assert.Throws<InstrumentDomainException>(() =>
            instrument.RestoreService(now.AddDays(2)));
    }

    [Fact]
    public void RetiredInstrument_ShouldNotBeUpdated()
    {
        var instrument = CreateInstrument();
        var now = DateTimeOffset.UtcNow;

        instrument.Retire(now.AddDays(1));

        Assert.Throws<InstrumentDomainException>(() =>
            instrument.UpdateInformation(
                "Updated HPLC",
                InstrumentType.Chromatograph,
                "Agilent",
                "New Model",
                "NEW-SERIAL",
                "Room 10",
                null,
                now.AddDays(2)));
    }
}
