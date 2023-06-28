using System.Collections.ObjectModel;

namespace Services.Logging
{
    public class LoggingService
    {
        private LoggingService() { }

        private static LoggingService? mInstance = null;
        public static LoggingService Instance
        {
            get
            {
                mInstance ??= new LoggingService();
                return mInstance;
            }
        }

        public ObservableCollection<LogItem> LogItemCollection { get; set; } = new();

        public int LogCount
        {
            get => LogItemCollection.Count;
        }
        
        public void AddLog(string from, string message) => AddLog(new(DateTime.Now, from, message));

        public void AddLog(LogItem log) => LogItemCollection.Add(log);

        public void RemoveLog(LogItem log)
        {
            if (LogItemCollection.Contains(log))
                LogItemCollection.Remove(log);
        }

        public void ClearLog() => LogItemCollection.Clear();
    }

    public record LogItem(DateTime Timestamp, string From, string Message);
}