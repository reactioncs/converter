using System;
using System.Collections.Generic;
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
            LogItemCollection.CollectionChanged += (o, e) => RemoveAllCommand.NotifyCanExecuteChanged();

            LogItemCollection.Add(new(DateTime.Now, "LoggingViewModel", "Platform and Runtime Independent"));
            LogItemCollection.Add(new(DateTime.Now, "LoggingViewModel", "Simple to pick-up and use\n Reference Implementation"));
        }

        [RelayCommand(CanExecute = nameof(IsRemoveAllAllCanExecute))]
        private void RemoveAll()
        {
            LogItemCollection.Clear();
        }
        private bool IsRemoveAllAllCanExecute() => LogItemCollection.Count > 0;

        [RelayCommand]
        private void RemoveSelected()
        {
            throw new NotImplementedException();
        }

        private readonly List<string> randomMessages = new()
        {
            "The Car Turned The Corner.",
            "Did You Know That, Along With Gorgeous Architecture, It's Home To The Largest Tamale ?",
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

        [RelayCommand]
        private void AddRandom()
        {
            string randomMessage = randomMessages[new Random().Next() % randomMessages.Count];
            LogItemCollection.Add(new(DateTime.Now, "LoggingViewModel", randomMessage));
        }
    }
}