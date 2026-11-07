using LIMS.Domain.Instruments;
using LIMS.Domain.Instruments.Enums;

namespace LIMS.UnitTests.Domain.Instruments;

public sealed class InstrumentTests
{
    private static readonly Guid LaboratoryId =
        Guid.Parse("11111111-1111-1111-1111-111111111111");

    private static Instrument CreateInstrument(
        DateTimeOffset? registeredAt = null)
    {
        return Instrument.Register(
            LaboratoryId,
            "NMR-001",
            "NMR Spectrometer",
            InstrumentType.NmrSpectrometer,
            registeredAt ?? DateTimeOffset.UtcNow,
            "Bruker",
            "AVANCE III",
            "SN-12345",
            "Chemistry Laboratory",
            "300 MHz spectrometer");
    }

    [Fact]
    public void Register_ShouldCreateAvailableInstrument()
    {
        var registeredAt = DateTimeOffset.UtcNow;

        var instrument = CreateInstrument(registeredAt);

        Assert.NotEqual(Guid.Empty, instrument.Id);
        Assert.Equal(LaboratoryId, instrument.LaboratoryId);
        Assert.Equal("NMR-001", instrument.InstrumentCode);
        Assert.Equal("NMR Spectrometer", instrument.Name);
        Assert.Equal(InstrumentType.NmrSpectrometer, instrument.Type);
        Assert.Equal(InstrumentStatus.Available, instrument.Status);
        Assert.Equal(registeredAt, instrument.RegisteredAt);
        Assert.Single(instrument.DomainEvents);
    }

    [Fact]
    public void Register_ShouldNormalizeInstrumentCode()
    {
        var instrument = Instrument.Register(
            LaboratoryId,
            "  nmr-001  ",
            "NMR Spectrometer",
            InstrumentType.NmrSpectrometer,
            DateTimeOffset.UtcNow,
            null,
            null,
            null,
            null,
            null);

        Assert.Equal("NMR-001", instrument.InstrumentCode);
    }

    [Fact]
    public void Register_ShouldThrow_WhenLaboratoryIdIsEmpty()
    {
        Assert.Throws<InstrumentDomainException>(() =>
            Instrument.Register(
                Guid.Empty,
                "NMR-001",
                "NMR Spectrometer",
                InstrumentType.NmrSpectrometer,
                DateTimeOffset.UtcNow,
                null,
                null,
                null,
                null,
                null));
    }

    [Fact]
    public void Register_ShouldThrow_WhenCodeIsInvalid()
    {
        Assert.Throws<InstrumentDomainException>(() =>
            Instrument.Register(
                LaboratoryId,
                "NMR 001",
                "NMR Spectrometer",
                InstrumentType.NmrSpectrometer,
                DateTimeOffset.UtcNow,
                null,
                null,
                null,
                null,
                null));
    }

    [Fact]
    public void UpdateInformation_ShouldUpdateInstrumentDetails()
    {
        var instrument = CreateInstrument();
        var updatedAt = DateTimeOffset.UtcNow.AddMinutes(1);

        instrument.UpdateInformation(
            "Updated NMR",
            InstrumentType.NmrSpectrometer,
            "Bruker",
            "AVANCE NEO",
            "SN-99999",
            "Room 204",
            "Updated description",
            updatedAt);

        Assert.Equal("Updated NMR", instrument.Name);
        Assert.Equal("AVANCE NEO", instrument.Model);
        Assert.Equal("SN-99999", instrument.SerialNumber);
        Assert.Equal("Room 204", instrument.Location);
        Assert.Equal("Updated description", instrument.Description);
        Assert.Equal(updatedAt, instrument.UpdatedAt);
    }

    [Fact]
    public void MarkInUse_ShouldChangeStatusToInUse()
    {
        var instrument = CreateInstrument();
        var occurredOn = DateTimeOffset.UtcNow.AddMinutes(1);

        instrument.MarkInUse(occurredOn);

        Assert.Equal(InstrumentStatus.InUse, instrument.Status);
        Assert.Equal(occurredOn, instrument.UpdatedAt);
    }

    [Fact]
    public void MarkAvailable_ShouldChangeStatusToAvailable()
    {
        var instrument = CreateInstrument();
        instrument.MarkInUse(DateTimeOffset.UtcNow);

        var occurredOn = DateTimeOffset.UtcNow.AddMinutes(1);
        instrument.MarkAvailable(occurredOn);

        Assert.Equal(InstrumentStatus.Available, instrument.Status);
        Assert.Equal(occurredOn, instrument.UpdatedAt);
    }

    [Fact]
    public void MarkInUse_ShouldThrow_WhenInstrumentIsNotAvailable()
    {
        var instrument = CreateInstrument();
        instrument.MarkInUse(DateTimeOffset.UtcNow);

        Assert.Throws<InstrumentDomainException>(() =>
            instrument.MarkInUse(DateTimeOffset.UtcNow.AddMinutes(1)));
    }

    [Fact]
    public void MarkAvailable_ShouldThrow_WhenInstrumentIsNotInUse()
    {
        var instrument = CreateInstrument();

        Assert.Throws<InstrumentDomainException>(() =>
            instrument.MarkAvailable(DateTimeOffset.UtcNow));
    }

    [Fact]
    public void MarkOutOfService_ShouldChangeStatus()
    {
        var instrument = CreateInstrument();
        var occurredOn = DateTimeOffset.UtcNow.AddMinutes(1);

        instrument.MarkOutOfService(
            "Unexpected technical failure",
            occurredOn);

        Assert.Equal(InstrumentStatus.OutOfService, instrument.Status);
        Assert.Equal(occurredOn, instrument.UpdatedAt);
    }

    [Fact]
    public void RestoreService_ShouldReturnInstrumentToAvailable()
    {
        var instrument = CreateInstrument();
        instrument.MarkOutOfService(
            "Unexpected technical failure",
            DateTimeOffset.UtcNow);

        instrument.RestoreService(DateTimeOffset.UtcNow.AddMinutes(1));

        Assert.Equal(InstrumentStatus.Available, instrument.Status);
    }

    [Fact]
    public void Retire_ShouldSetRetiredStatusAndTimestamp()
    {
        var instrument = CreateInstrument();
        var retiredAt = DateTimeOffset.UtcNow.AddDays(1);

        instrument.Retire(retiredAt);

        Assert.Equal(InstrumentStatus.Retired, instrument.Status);
        Assert.Equal(retiredAt, instrument.RetiredAt);
    }

    [Fact]
    public void Retire_ShouldThrow_WhenInstrumentIsInUse()
    {
        var instrument = CreateInstrument();
        instrument.MarkInUse(DateTimeOffset.UtcNow);

        Assert.Throws<InstrumentDomainException>(() =>
            instrument.Retire(DateTimeOffset.UtcNow.AddMinutes(1)));
    }
}
