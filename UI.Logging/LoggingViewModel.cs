using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Services.Logging;

namespace UI.Logging
{
    public partial class LoggingViewModel: ObservableObject
    {
        public LoggingService LoggingService { get; set; } = LoggingService.Instance;

        public LoggingViewModel()
        {
            LoggingService.LogItemCollection.CollectionChanged += (o, e) => RemoveAllCommand.NotifyCanExecuteChanged();

            LoggingService.AddLog("LoggingViewModel", GetRandomMessage());
            LoggingService.AddLog("LoggingViewModel", GetRandomMessage());
        }

        [RelayCommand(CanExecute = nameof(IsRemoveAllAllCanExecute))]
        private void RemoveAll()
        {
            LoggingService.ClearLog();
        }
        private bool IsRemoveAllAllCanExecute() => LoggingService.LogCount > 0;

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
                LoggingService.RemoveLog(log);
            }
        }

        private readonly List<string> randomMessages = new()
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

        public string GetRandomMessage() => randomMessages[new Random().Next() % randomMessages.Count];

        [RelayCommand]
        private void AddRandom()
        {
            LoggingService.AddLog("LoggingViewModel", GetRandomMessage());
        }
    }
}