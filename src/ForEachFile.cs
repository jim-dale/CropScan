
namespace CropScan
{
    using System;
    using System.Linq;
    using Microsoft.Extensions.FileSystemGlobbing;

    public class ForEachFile
    {
        public void Run(string path, string include, string exclude, Action<string> action)
        {
            var matcher = new Matcher(StringComparison.OrdinalIgnoreCase);

            if (string.IsNullOrEmpty(include))
            {
                matcher.AddInclude("**/*");
            }
            else
            {
                var tmp = include.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                matcher.AddIncludePatterns(tmp);
            }
            if (string.IsNullOrEmpty(exclude) == false)
            {
                var tmp = exclude.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                matcher.AddExcludePatterns(tmp);
            }

            var files = matcher.GetResultsInFullPath(path)
                            .ToList();

            if (files.Any())
            {
                foreach (var file in files)
                {
                    action?.Invoke(file);
                }
            }
        }
    }
}
