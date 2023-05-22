using Converter.Core;
using Converter.Model;
using System.IO;
using System;
using WebPWrapper;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using LibHeifSharp;
using System.Drawing;

namespace Converter
{
    internal class MainWindowViewModel : ObservableObject
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

        public string Trace
        {
            set
            {
                AddMessage(value);
            }
        }

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
            LogItem log = new LogItem() {
                Message = message,
                TimePrevious = timePrevious ?? DateTime.Now
            };

            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                LogItems.Add(log);
            }));
            return log.Time;
        }

        private static byte[] GenerateTestImage_16()
        {
            byte[] buffer = new byte[2097152];
            for (int i = 0; i < 1024; i++)
            {
                for (int j = 0; j < 1024; j++)
                {
                    byte c = j > 900 ? (byte)255 : (byte)(j / 8);
                    if (i < 100 && j < 100)
                        c = 200;
                    buffer[i * 2048 + j * 2 + 1] = c;
                }
            }
            return buffer;
        }

        private static byte[] GenerateTestImage_8()
        {
            byte[] buffer = new byte[1048576];
            for (int i = 0; i < 1024; i++)
            {
                for (int j = 0; j < 1024; j++)
                {
                    byte c = j > 900 ? (byte)255 : (byte)(j / 8);
                    if (i < 100 && j < 100)
                        c = 200;
                    buffer[i * 1024 + j] = c;
                }
            }
            return buffer;
        }

        private void BulkConvert(string path)
        {
            var img = ReadPngFile(path);

            string postfix = Quality > 100 ? "lossless" : $"{Quality}";
            string path0 = path.Replace(".png", $"_{postfix}.webp");
            string path1 = path.Replace(".png", $"_{postfix}.avif");
            string path2 = path.Replace(".png", $"_{postfix}.heif");

            var t = DateTime.Now;
            var b0 = ConvertToWebpFormat(img.Bytes, img.Width, img.Height, 1, Quality);
            File.WriteAllBytes(path0, b0);
            int b0_size = b0.Length / 1024; 
            AddMessage($"{b0_size}KB   {path0}", t);

            t = DateTime.Now;
            var b1 = ConvertToAvifFormat(img.Bytes, img.Width, img.Height, 1, Quality);
            File.WriteAllBytes(path1, b1);
            int b1_size = b1.Length / 1024; 
            AddMessage($"{b1_size}KB   {path1}", t);

            t = DateTime.Now;
            var b2 = ConvertToHeifFormat(img.Bytes, img.Width, img.Height, 1, Quality);
            File.WriteAllBytes(path2, b2);
            int b2_size = b2.Length / 1024; 
            AddMessage($"{b2_size}KB   {path2}", t);
        }

        private void MutiAvifTest(int id)
        {
            Random r = new Random();
            ImageBuffer img = ReadPngFile("C:/Users/Dell/Desktop/test.png");

            for (int i = 0; i < 20; i++)
            {
                ImageBuffer img_i = img;
                int s = r.Next(100000, 500000);
                for (int j = s; j < s + 5000; j++)
                {
                    if (j > img_i.Bytes.Length + 10)
                        break;
                    img_i.Bytes[j] = (byte)(r.Next(100, 220));
                }
                var b = ConvertToAvifFormat(img.Bytes, img.Width, img.Height, 1, Quality);

                File.WriteAllBytes($"C:/Users/Dell/Desktop/test/{id}_{i}.avif", b);
                if (i % 10 == 9 || i == 0)
                    AddMessage($"{img_i.Bytes.Length}B   {id}_{i}.avif");
            }
        }

        public MainWindowViewModel()
        {
            FilePath = "C:/Users/zzz/Desktop";
            FileName = "test";
            LogItems = new ObservableCollection<LogItem>();

            SaveAsWEBPCommand = new RelayCommand(o => Task.Run(() => {
                var b = GenerateTestImage_8();
                var b_ = ConvertToWebpFormat(b, 1024, 1024, 1, 101);
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
                var img = ReadWebpFile("C:/Users/Dell/Desktop/tt/ori.webp");
                var b_ = ConvertToPngFormat(img.Bytes, img.Width, img.Height, 1);
                File.WriteAllBytes("C:/Users/Dell/Desktop/tt/res.png", b_);
                AddMessage("res.png");
            }));
            WriteTestCommand = new RelayCommand(o =>
            {
                int c = Environment.ProcessorCount / 5;
            });
            ClearCommand = new RelayCommand(o => LogItems.Clear());
        }

        public static byte[] ConvertToWebpFormat(byte[] bytes, int imageWidth, int imageHeight, int bytesPerPixel, int quality = 95)
        {
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
                        buffer_bitmap[(i * imageWidth + j) * 3]     = bytes[(i * imageWidth + j + 1) * bytesPerPixel - 1];
                        buffer_bitmap[(i * imageWidth + j) * 3 + 1] = bytes[(i * imageWidth + j + 1) * bytesPerPixel - 1];
                        buffer_bitmap[(i * imageWidth + j) * 3 + 2] = bytes[(i * imageWidth + j + 1) * bytesPerPixel - 1];
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
            PixelFormat f;
            switch (bytesPerPixel)
            {
                case 1:
                    f = PixelFormats.Gray8;
                    break;
                case 2:
                    f = PixelFormats.Gray16;
                    break;
                default:
                    f = PixelFormats.Gray8;
                    break;
            }
            var bitmap = BitmapSource.Create(imageWidth, imageHeight, 96, 96, f, null, bytes, imageWidth * bytesPerPixel);

            using (var stream = new MemoryStream())
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));
                encoder.Save(stream);
                return stream.ToArray();
            }
        }

        public static byte[] ConvertToHeifFormat(byte[] bytes, int imageWidth, int imageHeight, int bytesPerPixel, int quality = 95)
        {
            return ConvertToAvifFormat(bytes, imageWidth, imageHeight, bytesPerPixel, quality, false);
        }

        public static byte[] ConvertToAvifFormat(byte[] bytes, int imageWidth, int imageHeight, int bytesPerPixel, int quality = 95, bool isAVIF = true)
        {
            using (HeifContext context = new HeifContext())
            {
                var format = isAVIF ? HeifCompressionFormat.Av1 : HeifCompressionFormat.Hevc;
                var encoderDescriptors = context.GetEncoderDescriptors(format);
                var encoderDescriptor = encoderDescriptors[0];

                using (HeifEncoder encoder = context.GetEncoder(encoderDescriptor))
                {
                    HeifImage heifImage = new HeifImage(imageWidth, imageHeight, HeifColorspace.Monochrome, HeifChroma.Monochrome);

                    heifImage.AddPlane(HeifChannel.Y, imageWidth, imageHeight, 8);
                    var grayPlane = heifImage.GetPlane(HeifChannel.Y);
                    int grayPlaneStride = grayPlane.Stride;

                    if (bytesPerPixel == 1)
                    {
                        System.Runtime.InteropServices.Marshal.Copy(bytes, 0, grayPlane.Scan0, imageWidth * imageHeight);
                    }
                    else if (bytesPerPixel == 2)
                    {
                        int size = imageWidth * imageHeight;
                        var buffer = new byte[size];
                        for (int i = 0; i < size; i++)
                        {
                            buffer[i] = bytes[i * 2 + 1];
                        }
                        System.Runtime.InteropServices.Marshal.Copy(buffer, 0, grayPlane.Scan0, imageWidth * imageHeight);
                    }

                    heifImage.NclxColorProfile = new HeifNclxColorProfile(
                            ColorPrimaries.BT709,
                            TransferCharacteristics.Srgb,
                            MatrixCoefficients.BT601,
                            fullRange: true);

                    if (0 < quality && quality <= 100)
                        encoder.SetLossyQuality(quality);
                    else
                        encoder.SetLossless(true);

                    var encodingOptions = new HeifEncodingOptions { SaveAlphaChannel = false };

                    context.EncodeImage(heifImage, encoder, encodingOptions);

                    using (var stream = new MemoryStream())
                    {
                        context.WriteToStream(stream);
                        return stream.ToArray();
                    }
                }
            }
        }

        public static byte[] ReadHeifFile(string fileName)
        {
            using (HeifContext context = new HeifContext(fileName))
            {
                using (HeifImageHandle imageHandle = context.GetPrimaryImageHandle())
                {
                    using (var image = imageHandle.Decode(HeifColorspace.Rgb, HeifChroma.InterleavedRgb24))
                    {
                        int size = image.Width * image.Height;
                        byte[] bufferRGB = new byte[size * 3];
                        byte[] buffer = new byte[size];

                        HeifPlaneData heifPlaneData = image.GetPlane(HeifChannel.Interleaved);
                        System.Runtime.InteropServices.Marshal.Copy(heifPlaneData.Scan0, bufferRGB, 0, size * 3);
                        for (int i = 0; i < size; i++)
                        {
                            buffer[i] = bufferRGB[i * 3];
                        }
                        return buffer;
                    }
                }
            }
        }

        public ImageBuffer ReadPngFile(string path)
        {
            ImageBuffer imageBuffer = new ImageBuffer();

            FileStream fs = File.OpenRead(path);
            var img = (Bitmap)Image.FromStream(fs);

            imageBuffer.Width = img.Width;
            imageBuffer.Height = img.Height;
            byte[] buffer = new byte[imageBuffer.Width * imageBuffer.Height];

            for (int i = 0; i < imageBuffer.Height; i++)
            {
                for (int j = 0; j < imageBuffer.Width; j++)
                {
                    var c = img.GetPixel(i, j);
                    buffer[j * imageBuffer.Width + i] = c.R;
                }
            }
            imageBuffer.Bytes = buffer;
            return imageBuffer;
        }

        public ImageBuffer ReadWebpFile(string fileName)
        {
            ImageBuffer imageBuffer = new ImageBuffer();

            imageBuffer.Bytes = ReadWebpFile(fileName, out int width, out int height, out _);
            imageBuffer.Width = width;
            imageBuffer.Height = height;

            return imageBuffer;
        }

        public static byte[] ReadWebpFile(string fileName, out int width, out int height, out int bytesPerPixel)
        {
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                var image = new WebP().Decode(memoryStream.ToArray());

                width = image.Width;
                height = image.Height;
                bytesPerPixel = 1;

                int size = width * height;
                byte[] buffer = new byte[size];

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        buffer[i * width + j] = image.GetPixel(j, i).R;
                    }
                }
                return buffer;
            }
        }

        public struct ImageBuffer
        {
            public int Width { get; set; }
            public int Height { get; set; }
            public byte[] Bytes { get; set; }
        }
    }
}
