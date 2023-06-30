using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Services.Logging;

namespace UI.Logging
{
    public partial class LoggingViewModel: ObservableObject
    {
        public LoggingService Log { get; set; } = LoggingService.Instance;

        [ObservableProperty]
        public ICollectionView collectionView;

        public LoggingViewModel()
        {
            Log.OnLogItemCollectionChange += (o, e) =>
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    CollectionView?.Refresh();
                    RemoveAllCommand.NotifyCanExecuteChanged();
                });
            };

            CollectionView = CollectionViewSource.GetDefaultView(Log.LogItemCollection);
            CollectionView.SortDescriptions.Add(new SortDescription("Timestamp", ListSortDirection.Descending));

            Log.AddLog("LoggingView", GetRandomMessage());
            Log.AddLog("LoggingView", GetRandomMessage());
        }

        [RelayCommand(CanExecute = nameof(IsRemoveAllAllCanExecute))]
        private void RemoveAll() => Log.ClearLog();
        private bool IsRemoveAllAllCanExecute() => !CollectionView.IsEmpty;

        [RelayCommand]
        private void RemoveSelected(IList selectedLogs)
        {
            List<LogItem> logsToDelete = new();

            foreach (object log in selectedLogs)
            {
                logsToDelete.Add((LogItem)log);
            }

            foreach (LogItem log in logsToDelete)
            {
                Log.RemoveLog(log);
            }
        } 

        private readonly string[] randomMessages = new []
        {
            "The Car Turned The Corner.",
            "Did You Know That, Along With Gorgeous Architecture,\n It's Home To The Largest Tamale ?",
            "Nice To Meet You Too.+I Love Learning!",
            "The Cat Stretched.+I Ate Dinner.",
            "I'm Coming Right Now.",
            "Wouldn't It Be Lovely To Enjoy A Week Soaking Up The Culture ?",
            "I Feel Lethargic.+He Is At His Desk.",
            "I Can't Understand What He Wants Me To Do.",
            "Jacob Stood On His Tiptoes.",
            "To Drive A Car, You Need A License.",
            "There Is So Much To Understand.",
            "Of All The Places To Travel, Mexico Is At The Top Of My List.",
            "Tom Got Angry At The Children.",
            "They Had Trouble Finding The Place.",
            "We Had A Three-Course Meal.",
            "Would You Like To Travel With Me ?",
            "I'm Confident That I'll Win The Tennis Match.",
        };

        public string GetRandomMessage() => randomMessages[new Random().Next() % randomMessages.Length];

        [RelayCommand]
        private void AddRandom() => Log.AddLog("LoggingView", GetRandomMessage());
    }
}