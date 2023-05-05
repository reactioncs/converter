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

        public RelayCommand SaveAsWEBPCommand { get; set; }
        public RelayCommand SaveAsPNGCommand { get; set; }
        public RelayCommand SaveAsAVIFCommand { get; set; }
        public RelayCommand SaveAsHEIFCommand { get; set; }
        public RelayCommand TestCommand { get; set; }
        public RelayCommand ClearCommand { get; set; }

        public void AddMessage(string message)
        {
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                LogItems.Add(new LogItem() { Message = message });
            }));
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

        public MainWindowViewModel()
        {
            FilePath = "C:/Users/zzz/Desktop";
            FileName = "test";
            LogItems = new ObservableCollection<LogItem>();

            SaveAsWEBPCommand = new RelayCommand(o => Task.Run(() => {
                var b = GenerateTestImage_8();
                var b_ = ConvertToWebpFormat(b, 1024, 1024, 1, Quality);
                File.WriteAllBytes("C:/Users/zzz/Desktop/test.webp", b_);
                AddMessage("test.webp");
            }));
            SaveAsPNGCommand = new RelayCommand(o => Task.Run(() => {
                var b = GenerateTestImage_8();
                var b_ = ConvertToPngFormat(b, 1024, 1024, 1);
                File.WriteAllBytes("C:/Users/zzz/Desktop/test.png", b_);
                AddMessage("test.png");
            }));
            SaveAsAVIFCommand = new RelayCommand(o => Task.Run(() => {
                var b = GenerateTestImage_8();
                var b_ = ConvertToHEIFFormat(b, 1024, 1024, 1, Quality);
                File.WriteAllBytes($"C:/Users/zzz/Desktop/test.avif", b_);
                AddMessage($"test.avif");
            }));
            SaveAsHEIFCommand = new RelayCommand(o => Task.Run(() => {
                var b = GenerateTestImage_8();
                var b_ = ConvertToHEIFFormat(b, 1024, 1024, 1, Quality, false);
                File.WriteAllBytes($"C:/Users/zzz/Desktop/test.heif", b_);
                AddMessage($"test.heif");
            }));
            TestCommand = new RelayCommand(o => Task.Run(() => {
                var b_ = ConvertToPngFormat(HEIFRead("C:/Users/zzz/Desktop/test.heif"), 1024, 1024, 1);
                File.WriteAllBytes($"C:/Users/zzz/Desktop/test_c.png", b_);
                AddMessage($"test_c.png");
            }));
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

        // save as 8-bits-greyscale image no matter bytesPerPixel
        public static byte[] ConvertToHEIFFormat(byte[] bytes, int imageWidth, int imageHeight, int bytesPerPixel, int quality = 95, bool isAVIF = true)
        {
            using (HeifContext context = new HeifContext())
            {
                HeifCompressionFormat format = isAVIF ? HeifCompressionFormat.Av1 : HeifCompressionFormat.Hevc;
                var encoderDescriptors = context.GetEncoderDescriptors(format);
                HeifEncoderDescriptor encoderDescriptor = encoderDescriptors[0];

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

        public static byte[] HEIFRead(string fileName)
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
    }
}
