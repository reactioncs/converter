using ImageConverter.Core;
using CommunityToolkit.Mvvm;
using ImageConverter.Model;
using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ImageConverter
{
    public class MainWindowViewModel : ObservableObject
    {
        private int mQuality = 101;
        public int Quality
        {
            get => mQuality;
            set
            {
                mQuality = value;
                OnPropertyChanged();
            }
        }

        public List<string> AvailableBytesPerPixel { get; set; } = new() { "8-bits per pixel", "16-bits per pixel" };

        private string mSelectedBytesPerPixel;
        public string SelectedBytesPerPixel
        {
            get => mSelectedBytesPerPixel;
            set
            {
                mSelectedBytesPerPixel = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<LogItem> LogItems { get; set; }

        public RelayCommand OpenFolderCommand { get; set; }
        public RelayCommand ClearCommand { get; set; }
        public RelayCommand SaveGeneratedImageCommand { get; set; }
        public RelayCommand Test0Command { get; set; }
        public RelayCommand Test1Command { get; set; }

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

            Test0Command = new RelayCommand(o => Task.Run(() => { AddMessage("Test0"); }));
            Test1Command = new RelayCommand(o => Task.Run(() => { AddMessage("Test1"); }));

            SaveGeneratedImageCommand = new RelayCommand(o => Task.Run(() => { SaveGenerateImage(); }));

            OpenFolderCommand = new RelayCommand(o =>
            {
                string fileName = FileOperation.OpenImageFileDialog() ?? "null";
                AddMessage(fileName);
            });

            ClearCommand = new RelayCommand(o => LogItems.Clear());
        }

        private void SaveGenerateImage()
        {
            byte[] img_byte;

            switch(SelectedBytesPerPixel)
            {
                case "8-bits per pixel":
                    var img_8 = ImageGenerator.GenerateTestImage_8();
                    img_byte = ImageHelper.ConvertToPngFormat(img_8, 1024, 1024, 1);
                    break;
                case "16-bits per pixel":
                    var img_16 = ImageGenerator.GenerateTestImage_16();
                    img_byte = ImageHelper.ConvertToPngFormat(img_16, 1024, 1024, 2);
                    break;
                default:
                    AddMessage("Please select a format.");
                    return;
            }
            
            string? path = FileOperation.SavingImageFileDialog(ImageFormats.Png, title: "Save GenerateImage");
            if (path == null)
                return;

            File.WriteAllBytes(path, img_byte);
            AddMessage(path);
        }
    }
}
