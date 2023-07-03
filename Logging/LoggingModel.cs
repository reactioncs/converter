using System.Collections.Generic;
using System;
using System.ComponentModel;

namespace Logging
{
    public class LoggingModel
    {
        private LoggingModel() { }

        private static LoggingModel? mInstance = null;
        public static LoggingModel Instance
        {
            get
            {
                mInstance ??= new LoggingModel();
                return mInstance;
            }
        }

        public List<LogItem> LogItemCollection { get; set; } = new();
        public event CollectionChangeEventHandler? OnLogItemCollectionChange;

        public void AddLog(string from, string message) => AddLog(new(DateTime.Now, from, message));

        public void AddLog(LogItem log)
        {
            LogItemCollection.Add(log);
            OnLogItemCollectionChange?.Invoke(this, new(CollectionChangeAction.Add, null));
        }

        public void RemoveLog(LogItem log)
        {
            if (LogItemCollection.Contains(log))
                LogItemCollection.Remove(log);
            OnLogItemCollectionChange?.Invoke(this, new(CollectionChangeAction.Remove, null));
        }

        public void ClearLog()
        {
            LogItemCollection.Clear();
            OnLogItemCollectionChange?.Invoke(this, new(CollectionChangeAction.Refresh, null));
        }
    }

    public record LogItem(DateTime Timestamp, string From, string Message);
}