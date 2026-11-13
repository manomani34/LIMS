
using LIMS.Domain.Instruments;
using LIMS.Domain.Instruments.Enums;

namespace LIMS.UnitTests.Domain.Instruments;

public sealed class InstrumentMaintenanceTests
{
    private static readonly Guid LaboratoryId =
        Guid.Parse("11111111-1111-1111-1111-111111111111");

    private static Instrument CreateInstrument()
    {
        return Instrument.Register(
            LaboratoryId,
            "NMR-001",
            "NMR Spectrometer",
            InstrumentType.NmrSpectrometer,
            "Bruker",
            "AVANCE III",
            "SN-12345",
            "Chemistry Laboratory",
            "300 MHz spectrometer",
            DateTimeOffset.UtcNow);
    }

    [Fact]
    public void ScheduleMaintenance_ShouldCreateMaintenanceRecord()
    {
        var instrument = CreateInstrument();
        var scheduledAt = DateTimeOffset.UtcNow.AddDays(1);

        instrument.ScheduleMaintenance(
            MaintenanceType.Preventive,
            "Annual preventive maintenance",
            scheduledAt,
            "user-001",
            DateTimeOffset.UtcNow);

        Assert.Equal(InstrumentStatus.UnderMaintenance, instrument.Status);
        Assert.Single(instrument.MaintenanceRecords);

        var maintenance = instrument.MaintenanceRecords.Single();

        Assert.Equal(MaintenanceType.Preventive, maintenance.Type);
        Assert.Equal("Annual preventive maintenance", maintenance.Description);
        Assert.Equal(scheduledAt, maintenance.ScheduledAt);
        Assert.Equal("user-001", maintenance.ScheduledByUserId);
        Assert.Null(maintenance.CompletedAt);
    }

    [Fact]
    public void ScheduleMaintenance_ShouldThrow_WhenInstrumentIsInUse()
    {
        var instrument = CreateInstrument();
        instrument.MarkInUse(DateTimeOffset.UtcNow);

        Assert.Throws<InstrumentDomainException>(() =>
            instrument.ScheduleMaintenance(
                MaintenanceType.Preventive,
                "Maintenance",
                DateTimeOffset.UtcNow.AddDays(1),
                "user-001",
                DateTimeOffset.UtcNow));
    }

    [Fact]
    public void ScheduleMaintenance_ShouldThrow_WhenActiveMaintenanceAlreadyExists()
    {
        var instrument = CreateInstrument();
        var now = DateTimeOffset.UtcNow;

        instrument.ScheduleMaintenance(
            MaintenanceType.Preventive,
            "First maintenance",
            now.AddDays(1),
            "user-001",
            now);

        Assert.Throws<InstrumentDomainException>(() =>
            instrument.ScheduleMaintenance(
                MaintenanceType.Inspection,
                "Second maintenance",
                now.AddDays(2),
                "user-002",
                now.AddMinutes(1)));
    }

    [Fact]
    public void CompleteMaintenance_ShouldCompleteRecord()
    {
        var instrument = CreateInstrument();
        var now = DateTimeOffset.UtcNow;

        instrument.ScheduleMaintenance(
            MaintenanceType.Calibration,
            "Calibration",
            now.AddDays(1),
            "user-001",
            now);

        var maintenanceId = instrument.MaintenanceRecords.Single().Id;
        var completedAt = now.AddDays(2);

        instrument.CompleteMaintenance(
            maintenanceId,
            "user-002",
            "Calibration completed successfully",
            completedAt);

        var maintenance = instrument.MaintenanceRecords.Single();

        Assert.Equal(completedAt, maintenance.CompletedAt);
        Assert.Equal("user-002", maintenance.CompletedByUserId);
        Assert.Equal(
            "Calibration completed successfully",
            maintenance.CompletionNotes);
        Assert.Equal(InstrumentStatus.Available, instrument.Status);
    }

    [Fact]
    public void CompleteMaintenance_ShouldThrow_WhenMaintenanceDoesNotExist()
    {
        var instrument = CreateInstrument();

        Assert.Throws<InstrumentDomainException>(() =>
            instrument.CompleteMaintenance(
                Guid.NewGuid(),
                "user-001",
                null,
                DateTimeOffset.UtcNow));
    }

    [Fact]
    public void CompleteMaintenance_ShouldThrow_WhenAlreadyCompleted()
    {
        var instrument = CreateInstrument();
        var now = DateTimeOffset.UtcNow;

        instrument.ScheduleMaintenance(
            MaintenanceType.Preventive,
            "Maintenance",
            now.AddDays(1),
            "user-001",
            now);

        var maintenanceId = instrument.MaintenanceRecords.Single().Id;

        instrument.CompleteMaintenance(
            maintenanceId,
            "user-002",
            null,
            now.AddDays(2));

        Assert.Throws<InstrumentDomainException>(() =>
            instrument.CompleteMaintenance(
                maintenanceId,
                "user-003",
                null,
                now.AddDays(3)));
    }
}
