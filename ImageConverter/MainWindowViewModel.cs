using ImageConverter.Model;
using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ImageConverter
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private int quality = 101;
        [ObservableProperty]
        private string selectedBytesPerPixel = "8-bits per pixel";
        public List<string> AvailableBytesPerPixel { get; set; } = new() { "8-bits per pixel", "16-bits per pixel" };
        public ObservableCollection<LogItem> LogItems { get; set; } = new ();

        [RelayCommand]
        public void Test0() => Task.Run(() => { AddMessage("Test0"); });
        [RelayCommand]
        public void Test1() => Task.Run(() => { AddMessage("Test1"); });
        [RelayCommand]
        public void SaveGeneratedImage() => Task.Run(() => { SaveGenerateImage(); });
        [RelayCommand]
        public void OpenFolder()
        {
            string fileName = FileOperationInstance.OpenImageFileDialog() ?? "null";
            AddMessage(fileName);
        }
        [RelayCommand]
        public void Clear() => LogItems.Clear();

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

        public FileOperation FileOperationInstance { get; set; } = FileOperation.Instance;

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
            
            string? path = FileOperationInstance.SavingImageFileDialog(ImageFormats.Png, title: "Save GenerateImage");
            if (path == null)
                return;

            File.WriteAllBytes(path, img_byte);
            AddMessage(path);
        }
    }
}
