namespace LIMS.Api.Configuration;

public static class LoggingConfiguration
{
    public static ILoggingBuilder AddLimsLogging(
        this ILoggingBuilder logging)
    {
        logging.ClearProviders();

        logging.AddJsonConsole(options =>
        {
            options.IncludeScopes = true;
            options.TimestampFormat = "yyyy-MM-dd HH:mm:ss.fff ";
            options.UseUtcTimestamp = true;
        });

        return logging;
    }
}
