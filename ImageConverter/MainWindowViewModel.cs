using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Logging;

namespace ImageConverter
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private int quality = 101;
        [ObservableProperty]
        private string selectedBytesPerPixel = "8-bits per pixel";
        public List<string> AvailableBytesPerPixel { get; set; } = new() { "8-bits per pixel", "16-bits per pixel" };

        public LoggingModel Log { get; set; } = LoggingModel.Instance;

        [RelayCommand]
        public void Test0() => Task.Run(() => { Log.AddLog("MainWindowView", "Test0"); });
        [RelayCommand]
        public void Test1() => Task.Run(() => { Log.AddLog("MainWindowView", "Test1"); });
        [RelayCommand]
        public async Task SaveGeneratedImageAsync() => await Task.Run(() => { SaveGenerateImage(); });
        [RelayCommand]
        public void OpenFolder()
        {
            string fileName = FileOperationInstance.OpenImageFileDialog() ?? "null";
            Log.AddLog("MainWindowView", fileName);
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
                    Log.AddLog("MainWindowView", "Please select a format.");
                    return;
            }
            
            string? path = FileOperationInstance.SavingImageFileDialog(ImageFormats.Png, title: "Save GenerateImage");
            if (path == null)
                return;

            File.WriteAllBytes(path, img_byte);
            Log.AddLog("MainWindowView", path);
        }
    }
}
