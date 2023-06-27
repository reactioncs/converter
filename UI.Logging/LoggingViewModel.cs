using System;
using System.Collections.ObjectModel;
using Services.Logging;

namespace UI.Logging
{
    public class LoggingViewModel
    {
        public ObservableCollection<LogItem> LogItemCollection { get; set; } = new();

        public LoggingViewModel()
        {
            LogItemCollection.Add(new(DateTime.Now, "LoggingViewModel", "Test0"));
            LogItemCollection.Add(new(DateTime.Now, "LoggingViewModel", "Test1"));
            LogItemCollection.Add(new(DateTime.Now, "LoggingViewModel", "Test2"));
            LogItemCollection.Add(new(DateTime.Now, "LoggingViewModel", "Test3"));
            LogItemCollection.Add(new(DateTime.Now, "LoggingViewModel", "Test3"));
            LogItemCollection.Add(new(DateTime.Now, "LoggingViewModel", "Test3"));
            LogItemCollection.Add(new(DateTime.Now, "LoggingViewModel", "Test3"));
            LogItemCollection.Add(new(DateTime.Now, "LoggingViewModel", "Test3"));
            LogItemCollection.Add(new(DateTime.Now, "LoggingViewModel", "Test3"));
            LogItemCollection.Add(new(DateTime.Now, "LoggingViewModel", "Test3"));
            LogItemCollection.Add(new(DateTime.Now, "LoggingViewModel", "Test3"));
        }
    }
}
