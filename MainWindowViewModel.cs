using Converter.Core;
using Converter.Model;
using System.Drawing;
using System.IO;
using System;
using WebPWrapper;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Runtime.InteropServices;

namespace Converter
{
    internal class MainWindowViewModel : ObservableObject
    {
        private int mQuality = 95;
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

        public string Trace
        {
            set
            {
                AddMessage(value);
            }
        }

        public string FilePath { get; set; }
        public string FileName { get; set; }

        public RelayCommand SavrAsWEBPCommand { get; set; }
        public RelayCommand SavrAsAVIFCommand { get; set; }
        public RelayCommand SavrAsPNGCommand { get; set; }
        public RelayCommand TestCommand { get; set; }
        public RelayCommand ClearCommand { get; set; }

        public void AddMessage(string message)
        {
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                LogItems.Add(new LogItem() { Message = message });
            }));
        }

        private static byte[] GenerateTestImage()
        {
            byte[] buffer = new byte[2097152];
            for (int i = 0; i < 1024; i++)
            {
                for (int j = 0; j < 1024; j++)
                {
                    byte c = j > 900 ? (byte)255 : (byte)(j / 8);
                    if (i < 100 && j < 100)
                        c = 200;
                    buffer[i * 2048 + j] = c;
                }
            }
            return buffer;
        }

        public MainWindowViewModel()
        {
            FilePath = "C:/Users/zzz/Desktop";
            FileName = "test";
            LogItems = new ObservableCollection<LogItem>();

            SavrAsWEBPCommand = new RelayCommand(o => Task.Run(() => {
                var b = GenerateTestImage();
                var b_ = ConvertToWebpFormat(b, 1024, 1024, 2, 95);
                File.WriteAllBytes("C:/Users/zzz/Desktop/test.webp", b_);
                AddMessage("test.webp");
            }));
            SavrAsPNGCommand = new RelayCommand(o => Task.Run(() => {
                var b = GenerateTestImage();
                var b_ = ConvertToPngFormat(b, 1024, 1024, 2);
                File.WriteAllBytes("C:/Users/zzz/Desktop/test.png", b_);
                AddMessage("test.png");
            }));
            ClearCommand = new RelayCommand(o => LogItems.Clear());
        }

        public static byte[] ConvertToWebpFormat(byte[] bytes, int imageWidth, int imageHeight, int bytesPerPixel, int quality = 95)
        {
            int stride = imageWidth * bytesPerPixel;
            int size = imageWidth * imageHeight * 3;

            using (var bmp = new System.Drawing.Bitmap(imageWidth, imageHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb))
            {
                System.Drawing.Imaging.BitmapData data = bmp.LockBits(
                new System.Drawing.Rectangle(System.Drawing.Point.Empty, bmp.Size),
                System.Drawing.Imaging.ImageLockMode.WriteOnly,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb
            );

                var buffer_bitmap = new byte[size];
                for (int i = 0; i < imageWidth; i++)
                {
                    for (int j = 0; j < imageHeight; j++)
                    {
                        buffer_bitmap[(i * imageWidth + j) * 3] = bytes[i * stride + j];
                        buffer_bitmap[(i * imageWidth + j) * 3 + 1] = bytes[i * stride + j];
                        buffer_bitmap[(i * imageWidth + j) * 3 + 2] = bytes[i * stride + j];
                    }
                }
                System.Runtime.InteropServices.Marshal.Copy(buffer_bitmap, 0, data.Scan0, size);
                bmp.UnlockBits(data);

                if (0 < quality && quality <= 100)
                    return new WebP().EncodeLossy(bmp, quality);
                else
                    return new WebP().EncodeLossless(bmp);
            }
        }

        public static byte[] ConvertToPngFormat(byte[] bytes, int imageWidth, int imageHeight, int bytesPerPixel)
        {
            var bitmap = BitmapSource.Create(imageWidth, imageHeight, 96, 96, PixelFormats.Gray8, null, bytes, imageWidth * bytesPerPixel);

            using (var stream = new MemoryStream())
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));
                encoder.Save(stream);
                return stream.ToArray();
            }
        }
    }
}
