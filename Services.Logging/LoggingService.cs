namespace Services.Logging
{
    public class LoggingService
    {

    }

    public record LogItem(DateTime Timestamp, string From, string Message);
}