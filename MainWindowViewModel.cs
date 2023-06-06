using ImageConverter.Core;
using ImageConverter.Model;
using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ImageConverter
{
    public class MainWindowViewModel : ObservableObject
    {
        private int mQuality = 101;
        public int Quality
        {
            get { return mQuality; }
            set
            {
                mQuality = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<LogItem> LogItems { get; set; }

        public RelayCommand OpenFolderCommand { get; set; }
        public RelayCommand Test0Command { get; set; }
        public RelayCommand Test1Command { get; set; }
        public RelayCommand ClearCommand { get; set; }

        public DateTime AddMessage(string message, DateTime? timePrevious = null)
        {
            LogItem log = new()
            {
                Message = message,
                TimePrevious = timePrevious ?? DateTime.Now
            };

            System.Windows.Application.Current.Dispatcher.Invoke(() => LogItems.Add(log));

            return log.Time;
        }

        public MainWindowViewModel()
        {
            LogItems = new ObservableCollection<LogItem>();

            Test0Command = new RelayCommand(o => Task.Run(() => DoTest0()));
            Test1Command = new RelayCommand(o => Task.Run(() => SaveGenerateImageAsWebp()));

            OpenFolderCommand = new RelayCommand(o =>
            {
                string fileName = FileOperation.OpenImageFileDialog() ?? "null";
                AddMessage(fileName);
            });

            ClearCommand = new RelayCommand(o => LogItems.Clear());
        }
        private void DoTest0()
        {
        }

        private void SaveGenerateImageAsWebp()
        {
            var b = ImageGenerator.GenerateTestImage_8();
            var b_ = ImageHelper.ConvertToWebpFormat(b, 1024, 1024, 1, 101);

            string? path = FileOperation.SavingImageFileDialog(ImageFormats.Wepb, title: "Save GenerateImage As Webp");
            if (path == null)
                return;

            File.WriteAllBytes(path, b_);
            AddMessage(path);
        }
    }
}
