using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;

namespace ImageConverter
{
    public class FileOperation
    {
        private FileOperation() { }

        private static FileOperation? mInstance = null;
        public static FileOperation Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = new FileOperation();
                }
                return mInstance;
            }
        }

        public string InitialDirectory { get; set; } = "C:\\";

        public string? OpenImageFileDialog(string title = "Chosing Image")
        {
            OpenFileDialog fileDialog = new()
            {
                InitialDirectory = InitialDirectory,
                Filter = "Png File|*.png|Wepb File|*.webp|Avif File|*.avif|Heif File|*.heif|All files|*.*",
                FilterIndex = 5,
                Title = title
            };

            if (fileDialog.ShowDialog() == true)
            {
                InitialDirectory = fileDialog.FileName;
                return fileDialog.FileName;
            }

            return null;
        }

        public string? SavingImageFileDialog(ImageFormats f, string title = "Saving Image")
        {
            string filter = "";
            filter += f.HasFlag(ImageFormats.Png) ? "Png File|*.png|" : "";
            filter += f.HasFlag(ImageFormats.Wepb) ? "Wepb File|*.webp|" : "";
            filter += f.HasFlag(ImageFormats.Avif) ? "Avif File|*.avif|" : "";
            filter += f.HasFlag(ImageFormats.Heif) ? "Heif File|*.heif|" : "";

            SaveFileDialog fileDialog = new()
            {
                InitialDirectory = InitialDirectory,
                // get rid of the last '|'
                Filter = filter[..^1],
                Title = title
            };

            if (fileDialog.ShowDialog() == true)
            {
                InitialDirectory = fileDialog.FileName;
                return fileDialog.FileName;
            }

            return null;
        }

        public static bool OpenInFileExplorer(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                ProcessStartInfo startInfo = new()
                {
                    Arguments = folderPath,
                    FileName = "explorer.exe"
                };

                Process.Start(startInfo);
                return true;
            }
            return false;
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
