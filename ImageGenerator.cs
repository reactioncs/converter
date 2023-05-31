using System;

namespace ImageConverter
{
    public static class ImageGenerator
    {
        public static byte[] GenerateTestImage_16()
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

        public static byte[] GenerateTestImage_8()
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
    }
}
