using System;
using System.IO;

namespace AtlasReaper.Utils
{
    class FileUtils
    {
        internal static bool CanWriteToDirectory(string directoryPath)
        {
            try
            {
                string testFilePath = Path.Combine(directoryPath, Guid.NewGuid().ToString() + ".tmp");
                using (FileStream fileStream = File.Create(testFilePath))
                {
                    fileStream.Close();
                }
                File.Delete(testFilePath);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        internal static string GetFileName(string filePath)
        {
            string fullPath;
            if (Path.IsPathRooted(filePath))
            {
                fullPath = Path.GetFullPath(filePath);
            }
            else
            {
                string currentDirectory = Environment.CurrentDirectory;
                fullPath = Path.Combine(currentDirectory, filePath);
                Console.WriteLine(fullPath);
            }
            string directory = Path.GetDirectoryName(fullPath);
            string fileName = Path.GetFileName(fullPath);

            if (File.Exists(fullPath))
            {
                Console.WriteLine("File already exists. Please choose a different file name.");
                return fullPath;
            }

            if (!Directory.Exists(directory))
            {
                Console.WriteLine("Invalid directory. Please specify a valid directory.");
                return fullPath;
            }

            if (!FileUtils.CanWriteToDirectory(directory))
            {
                Console.WriteLine("Unable to write to the specified directory. Please choose a different location.");
                return fullPath;
            }

            return fullPath;
        }
    }
}
