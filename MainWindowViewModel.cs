using ImageConverter.Core;
using ImageConverter.Model;
using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Win32;

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
                string fileName = DoOpenFileDialog() ?? "null";
                AddMessage(fileName);
            });

            ClearCommand = new RelayCommand(o => LogItems.Clear());
        }
        private void DoTest0()
        {
        }

        private static string? DoOpenFileDialog(string title = "saving", string initialDirectory = "C:\\")
        {
            OpenFileDialog fileDialog = new()
            {
                InitialDirectory = initialDirectory,
                Filter = "Png File|*.png|Wepb File|*.webp|Avif File|*.avif|Heif File|*.heif",
                Title = title
            };

            if (fileDialog.ShowDialog() == true)
                return fileDialog.FileName;

            return null;
        }

        private static string? DoSaveFileDialog(Format f, string title = "saving", string initialDirectory = "C:\\")
        {
            string filter = "";
            filter += f.HasFlag(Format.Png) ? "Png File|*.png|" : "";
            filter += f.HasFlag(Format.Wepb) ? "Wepb File|*.webp|" : "";
            filter += f.HasFlag(Format.Avif) ? "Avif File|*.avif|" : "";
            filter += f.HasFlag(Format.Heif) ? "Heif File|*.heif|" : "";

            SaveFileDialog fileDialog = new()
            {
                InitialDirectory = initialDirectory,
                // get rid of the last '|'
                Filter = filter[..^1],
                Title = title
            };

            if (fileDialog.ShowDialog() == true)
                return fileDialog.FileName;

            return null;
        }

        private void SaveGenerateImageAsWebp()
        {
            var b = ImageGenerator.GenerateTestImage_8();
            var b_ = ImageHelper.ConvertToWebpFormat(b, 1024, 1024, 1, 101);

            string? path = DoSaveFileDialog(Format.Wepb, title: "Save GenerateImage As Webp");
            if (path == null)
                return;

            File.WriteAllBytes(path, b_);
            AddMessage(path);
        }
    }

    [Flags]
    public enum Format
    {
        None = 0,
        Png = 1,
        Wepb = 2,
        Avif = 4,
        Heif = 8
    }
}
