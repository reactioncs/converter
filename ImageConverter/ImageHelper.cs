using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Drawing;
using LibHeifSharp;
using ThirdParty.LibWebp;
using System;

namespace ImageConverter
{
    public class ImageHelper
    {
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
                        buffer_bitmap[(i * imageWidth + j) * 3] = bytes[(i * imageWidth + j + 1) * bytesPerPixel - 1];
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
                default:
                    f = PixelFormats.Gray8;
                    break;
                case 2:
                    f = PixelFormats.Gray16;
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

        public static ImageBuffer ReadPngFile(string fileName)
        {
            ImageBuffer imageBuffer = new ImageBuffer();

            imageBuffer.Bytes = ReadPngFile(fileName, out int width, out int height, out _);
            imageBuffer.Width = width;
            imageBuffer.Height = height;

            return imageBuffer;
        }

        public static byte[] ReadPngFile(string fileName, out int width, out int height, out int bytesPerPixel)
        {
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                var decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                var bitmap = decoder.Frames[0];

                width = bitmap.PixelWidth;
                height = bitmap.PixelHeight;
                bytesPerPixel = (bitmap.Format.BitsPerPixel + 7) / 8;

                var pixels = new byte[height * width * bytesPerPixel];

                bitmap.CopyPixels(pixels, width * bytesPerPixel, 0);

                return pixels;
            }
        }

        public static ImageBuffer ReadWebpFile(string fileName)
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
                byte[] bufferRGB = new byte[size * 3];
                System.Drawing.Imaging.BitmapData data = image.LockBits(
                    new Rectangle(Point.Empty, image.Size),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly,
                    image.PixelFormat
                );
                System.Runtime.InteropServices.Marshal.Copy(data.Scan0, bufferRGB, 0, size * 3);

                for (int i = 0; i < size; i++)
                {
                    buffer[i] = bufferRGB[i * 3];
                }
                return buffer;
            }
        }
    }

    public struct ImageBuffer
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public byte[] Bytes { get; set; }
    }
}
