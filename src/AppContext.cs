
namespace CropScan
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class AppContext
    {
        public bool ShowHelp { get; set; }
        public bool WhatIf { get; set; }
        public decimal DefaultMinResX { get; set; }
        public decimal DefaultMinResY { get; set; }
        public decimal? WidthCm { get; set; }
        public decimal? HeightCm { get; set; }
        public string FileNameSuffix { get; set; }
        public string BasePath { get; set; }
        public string SearchPatterns { get; set; }
        public bool Faulted => Faults.Any();
        public List<string> Faults { get; set; }

        public AppContext()
        {
            Faults = new List<string>();
            DefaultMinResX = Constants.DefaultMinResolution;
            DefaultMinResY = Constants.DefaultMinResolution;
        }

        public void Verify()
        {
            if (string.IsNullOrEmpty(SearchPatterns))
            {
                AddFault("At least one file or file specification must be supplied");
            }
            if (WidthCm.HasValue == false && HeightCm.HasValue == false)
            {
                AddFault("At least the Width or Height must be specified");
            }
            if (DefaultMinResX < Constants.MinAllowedResolution || DefaultMinResY < Constants.MinAllowedResolution)
            {
                AddFault($"The minimum resolution is {Constants.MinAllowedResolution}dpi");
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
            Faults.Add(message);
        }

        public void ShowFaults()
        {
            foreach (var message in Faults)
            {
                Console.WriteLine(message);
            }
            Console.WriteLine();
        }

        public static void ShowHelpText()
        {
            Console.WriteLine("Crops an image file given new height and / or width measurements.");
            Console.WriteLine("The measurements can be specified in centimetres (cm) or inches (in).");
            Console.WriteLine();
            Console.WriteLine("CropScan [-?] [-w width] [-h height] [-s suffix] [-wi] filespec[;filespec]");
            Console.WriteLine();
            Console.WriteLine("  -?             Show this help information");
            Console.WriteLine("  -w width       New width for the cropped image");
            Console.WriteLine("  -h height      New height for the cropped image");
            Console.WriteLine("  -s suffix      Suffix to add to the filename to create a copy of the input file");
            Console.WriteLine("  -wi            Displays a message that describes the effect of the command, instead of executing the command");
            Console.WriteLine();
            Console.WriteLine("  [filespec]");
            Console.WriteLine("                 File or files specification");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("CropScan -h 14.85cm *.jpg");
            Console.WriteLine("CropScan -h 14.85cm -w 10in *.jpg;*.png");
            Console.WriteLine();
        }
    }
}
