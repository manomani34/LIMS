namespace LIMS.Contracts.Instruments.Requests;

public sealed record ScheduleInstrumentMaintenanceRequest(
    MaintenanceTypeContract Type,
    string Description,
    DateTimeOffset ScheduledAt,
    string ScheduledByUserId);
