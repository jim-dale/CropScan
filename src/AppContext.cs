
namespace CropScan
{
    using System;
    using System.Collections.Generic;

    public class AppContext
    {
        public bool ShowHelp { get; set; }
        public bool WhatIf { get; set; }
        public decimal DefaultMinResX { get; set; }
        public decimal DefaultMinResY { get; set; }
        public decimal? WidthCm { get; set; }
        public decimal? HeightCm { get; set; }
        public string FileSuffix { get; set; }
        public List<string> SearchPaths { get; set; }
        public HashSet<string> Files { get; set; }
        public bool Faulted { get; set; }
        public List<string> Faults { get; set; }

        public AppContext()
        {
            SearchPaths = new List<string>();
            Files = new HashSet<string>();
            Faults = new List<string>();
            DefaultMinResX = Constants.DefaultMinResolution;
            DefaultMinResY = Constants.DefaultMinResolution;
        }

        public void Verify()
        {
            if (WidthCm.HasValue == false && HeightCm.HasValue == false)
            {
                AddFault("At least the cropped Width or Height must be specified.");
            }
            if (DefaultMinResX < Constants.MinResolution || DefaultMinResY < Constants.MinResolution)
            {
                AddFault($"The minimum resolution is {Constants.MinResolution}dpi.");
            }
        }

        public void AddInvalidArgFault(string arg)
        {
            AddFault($"Invalid option specified - \"{arg}\"");
        }

        public void AddExceptionFault(Exception exception)
        {
            AddFault(exception.Message);
        }

        public void AddFault(string message)
        {
            Faulted = true;
            Faults.Add(message);
        }

        public void ShowFaults()
        {
            if (Faulted)
            {
                foreach (var message in Faults)
                {
                    Console.WriteLine(message);
                }
                Console.WriteLine();
            }
        }
    }
}
