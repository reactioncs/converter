using Microsoft.Win32;
using System;

namespace ImageConverter
{
    public class FileOperation
    {
        public static string? OpenImageFileDialog(string title = "Chosing Image", string initialDirectory = "C:\\")
        {
            OpenFileDialog fileDialog = new()
            {
                InitialDirectory = initialDirectory,
                Filter = "Png File|*.png|Wepb File|*.webp|Avif File|*.avif|Heif File|*.heif|All files|*.*",
                FilterIndex = 5,
                Title = title
            };

            if (fileDialog.ShowDialog() == true)
                return fileDialog.FileName;

            return null;
        }

        public static string? SavingImageFileDialog(ImageFormats f, string title = "Saving Image", string initialDirectory = "C:\\")
        {
            string filter = "";
            filter += f.HasFlag(ImageFormats.Png) ? "Png File|*.png|" : "";
            filter += f.HasFlag(ImageFormats.Wepb) ? "Wepb File|*.webp|" : "";
            filter += f.HasFlag(ImageFormats.Avif) ? "Avif File|*.avif|" : "";
            filter += f.HasFlag(ImageFormats.Heif) ? "Heif File|*.heif|" : "";

            SaveFileDialog fileDialog = new()
            {
                InitialDirectory = initialDirectory,
                // get rid of the last '|'
                Filter = filter[..^1],
                Title = title
            };

            if (fileDialog.ShowDialog() == true)
                return fileDialog.FileName;

            return null;
        }

        public void OpenFolder(string path)
        {

        }
    }

    [Flags]
    public enum ImageFormats
    {
        Png = 1,
        Wepb = 2,
        Avif = 4,
        Heif = 8
    }
}
