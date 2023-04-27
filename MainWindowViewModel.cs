using Converter.Core;
using Converter.Model;
using System.Drawing;
using System.IO;
using System;
using WebPWrapper;
using OpenCvSharp;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
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

        public RelayCommand ConvertWithLibwebpCommand { get; set; }
        public RelayCommand ConvertWithCVCommand { get; set; }
        public RelayCommand TestCommand { get; set; }
        public RelayCommand ClearCommand { get; set; }

        public MainWindowViewModel()
        {
            FilePath = "C:/Users/zzz/Desktop";
            FileName = "test";
            LogItems = new ObservableCollection<LogItem>();

            ConvertWithLibwebpCommand = new RelayCommand(o => Task.Run(() => {
                for (int i = 0; i <= 10; i++)
                {
                    Thread.Sleep(200);
                    AddMessage("222");
                }
            }));
            ConvertWithCVCommand = new RelayCommand(o => ConvertWithCV(FilePath, FileName, Quality));
            ClearCommand = new RelayCommand(o => LogItems.Clear());
            TestCommand = new RelayCommand(o =>
            {
                var buffer = new byte[2097152];
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

                var bitmap = BitmapSource.Create(1024, 1024, 96, 96, PixelFormats.Gray8, null, buffer, 1024 * 2);

                //var b0 = ToBitmap(buffer);
                //b0.Save("C:/Users/zzz/Desktop/test.bmp");
                var b0 = new WebP().EncodeLossy(ToBitmap(buffer), 100);
                File.WriteAllBytes("C:/Users/zzz/Desktop/test.webp", b0);
                AddMessage("test.webp");

                var b1 = ConvertToPngFormat(bitmap);
                File.WriteAllBytes("C:/Users/zzz/Desktop/test.png", b1);
                AddMessage("test.png");
            });
        }

        public static System.Drawing.Bitmap ToBitmap(byte[] buffer)
        {
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(1024, 1024, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            System.Drawing.Imaging.BitmapData data = bmp.LockBits(
                new System.Drawing.Rectangle(System.Drawing.Point.Empty, bmp.Size),
                System.Drawing.Imaging.ImageLockMode.WriteOnly,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb
            );

            var buffer_bitmap = new byte[1024 * 1024 * 3];
            for (int i = 0; i < 1024; i++)
            {
                for (int j = 0; j < 1024; j++)
                {
                    buffer_bitmap[i * 3072 + j * 3] = buffer[i * 2048 + j];
                    buffer_bitmap[i * 3072 + j * 3 + 1] = buffer[i * 2048 + j];
                    buffer_bitmap[i * 3072 + j * 3 + 2] = buffer[i * 2048 + j];
                }
            }
            Marshal.Copy(buffer_bitmap, 0, data.Scan0, 1024 * 1024 * 3);
            bmp.UnlockBits(data);

            return bmp;
        }

        public static byte[] ConvertToPngFormat(BitmapSource bitmap)
        {
            using (var stream = new MemoryStream())
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));
                encoder.Save(stream);
                return stream.ToArray();
            }
        }

        public void AddMessage(string message)
        {
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                LogItems.Add(new LogItem() { Message = message });
            }));
        }

        public static void ConvertWithLibwebp(string inputPath, string outputPath, int quality)
        {
            Image img = Image.FromFile(inputPath);

            WebP webp = new WebP();

            if (quality > 100)
            {
                byte[] rawWebP = webp.EncodeLossless((Bitmap)img);
                File.WriteAllBytes(outputPath, rawWebP);
            }
            else if (quality > 0)
            {
                byte[] rawWebP = webp.EncodeLossy((Bitmap)img, quality);
                File.WriteAllBytes(outputPath, rawWebP);
            }
            else
                return;
        }

        public static void ConvertWithCV(string inputPath, string outputPath, int quality)
        {
            Mat mat = Cv2.ImRead(inputPath, ImreadModes.Color);

            Cv2.ImEncode(".webp", mat, out byte[] output, new int[] { (int)ImwriteFlags.WebPQuality, quality });

            File.WriteAllBytes(outputPath, output);
        }
    }
}
