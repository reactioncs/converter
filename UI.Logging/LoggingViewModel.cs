using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Services.Logging;

namespace UI.Logging
{
    public partial class LoggingViewModel: ObservableObject
    {
        public ObservableCollection<LogItem> LogItemCollection { get; set; } = new();

        public LoggingViewModel()
        {
            LogItemCollection.CollectionChanged += (o, e) => ClearAllCommand.NotifyCanExecuteChanged();

            LogItemCollection.Add(new(DateTime.Now, "LoggingViewModel", "Platform and Runtime Independent"));
            LogItemCollection.Add(new(DateTime.Now, "LoggingViewModel", "Simple to pick-up and use\n Reference Implementation"));
        }

        [RelayCommand(CanExecute = nameof(IsClearAllCanExecute))]
        private void ClearAll()
        {
            LogItemCollection.Clear();
        }
        private bool IsClearAllCanExecute() => LogItemCollection.Count > 0;

        [RelayCommand]
        private void RemoveSelected()
        {
            throw new NotImplementedException();
        }
    }
}
