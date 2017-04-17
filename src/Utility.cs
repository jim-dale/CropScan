
namespace CropScan
{
    using System;
    using System.IO;

    public struct DirectorySearchRequest
    {
        public string Directory { get; set; }
        public string SearchPattern { get; set; }

        public DirectorySearchRequest(string directory, string searchPattern)
        {
            Directory = directory;
            SearchPattern = searchPattern;
        }
    }

    public class Utility
    {
        public static DirectorySearchRequest GetDirectorySearchRequest(string path)
        {
            string directory = null;
            string searchPattern = "*";

            if (string.IsNullOrWhiteSpace(path) == false)
            {
                if (Directory.Exists(path))
                {
                    directory = path;
                }
                else
                {
                    directory = Path.GetDirectoryName(path);
                    searchPattern = Path.GetFileName(path);
                }
            }
            if (string.IsNullOrWhiteSpace(directory))
            {
                directory = Directory.GetCurrentDirectory();
            }
            return new DirectorySearchRequest(directory, searchPattern);
        }
    }
}
