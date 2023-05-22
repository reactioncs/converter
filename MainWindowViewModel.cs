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

        public string FilePath { get; set; }
        public string FileName { get; set; }

        public RelayCommand SaveAsWEBPCommand { get; set; }
        public RelayCommand SaveAsPNGCommand { get; set; }
        public RelayCommand SaveAsAVIFCommand { get; set; }
        public RelayCommand SaveAsHEIFCommand { get; set; }
        public RelayCommand TestCommand { get; set; }
        public RelayCommand WriteTestCommand { get; set; }
        public RelayCommand ClearCommand { get; set; }

        public DateTime AddMessage(string message, DateTime? timePrevious = null)
        {
            LogItem log = new LogItem()
            {
                Message = message,
                TimePrevious = timePrevious ?? DateTime.Now
            };

            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                LogItems.Add(log);
            }));
            return log.Time;
        }

        public MainWindowViewModel()
        {
            FilePath = "C:/Users/zzz/Desktop";
            FileName = "test";
            LogItems = new ObservableCollection<LogItem>();

            SaveAsWEBPCommand = new RelayCommand(o => Task.Run(() => {
                var b = ImageGenerator.GenerateTestImage_8();
                var b_ = ImageHelper.ConvertToWebpFormat(b, 1024, 1024, 1, 101);
                File.WriteAllBytes("C:/Users/Dell/Desktop/tt/ori.webp", b_);
                AddMessage("ori.webp");
            }));
            SaveAsPNGCommand = new RelayCommand(o => Task.Run(() => {
            }));
            SaveAsAVIFCommand = new RelayCommand(o => Task.Run(() => {
            }));
            SaveAsHEIFCommand = new RelayCommand(o => Task.Run(() => {
            }));
            TestCommand = new RelayCommand(o => Task.Run(() =>
            {
                var img = ImageHelper.ReadWebpFile("C:/Users/Dell/Desktop/tt/ori.webp");
                var b_ = ImageHelper.ConvertToPngFormat(img.Bytes, img.Width, img.Height, 1);
                File.WriteAllBytes("C:/Users/Dell/Desktop/tt/res.png", b_);
                AddMessage("res.webp");
            }));
            WriteTestCommand = new RelayCommand(o =>
            {
                int c = Environment.ProcessorCount / 5;
            });
            ClearCommand = new RelayCommand(o => LogItems.Clear());
        }
    }
}
