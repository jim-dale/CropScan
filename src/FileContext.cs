
namespace CropScan
{
    using System;
    using System.Drawing;
    using System.IO;

    public class FileContext
    {
        public string InputPath { get; set; }
        public decimal SrcResX { get; set; }
        public decimal SrcResY { get; set; }
        public int SrcWidthPx { get; set; }
        public int SrcHeightPx { get; set; }
        public string OutputPath { get; set; }
        public int OutWidthPx { get; set; }
        public int OutHeightPx { get; set; }
        public Rectangle CropRect { get; set; }
        public string Message { get; set; }

        public void FromSourceImage(Bitmap item, decimal? requiredWidthCm, decimal? requiredHeightCm)
        {
            SrcResX = Math.Round((decimal)item.HorizontalResolution, 2);
            SrcResY = Math.Round((decimal)item.VerticalResolution, 2);

            SrcWidthPx = item.Width;
            SrcHeightPx = item.Height;
            OutWidthPx = Utility.CalculateDimension(item.Width, SrcResX, requiredWidthCm);
            OutHeightPx = Utility.CalculateDimension(item.Height, SrcResY, requiredHeightCm);
            CropRect = new Rectangle(0, 0, OutWidthPx, OutHeightPx);
        }

        public void SetOutputFile(string path, string suffix)
        {
            string outputPath = path;

            if (string.IsNullOrEmpty(suffix) == false)
            {
                string directory = Path.GetDirectoryName(path);
                string name = Path.GetFileNameWithoutExtension(path);
                string ext = Path.GetExtension(path);

                name = name + suffix + ext;
                outputPath = Path.Combine(directory, name);
            }

            OutputPath = outputPath;
        }

        public string ToString(bool whatIf)
        {
            string prefix = (whatIf) ? "WhatIf: " : string.Empty;

            var srcWidthCm = Utility.ConvertPixelsToCms(SrcWidthPx, SrcResX);
            var srcHeightCm = Utility.ConvertPixelsToCms(SrcHeightPx, SrcResY);
            string srcDimensions = $"{srcWidthCm}x" + $"{srcHeightCm}cm (WxH) ";

            var outWidthCm = Utility.ConvertPixelsToCms(OutWidthPx, SrcResX);
            var outHeightCm = Utility.ConvertPixelsToCms(OutHeightPx, SrcResY);
            string outDimensions = $"{outWidthCm}x" + $"{outHeightCm}cm (WxH) ";

            return prefix + $"\"{InputPath}\" {srcDimensions} => \"{OutputPath}\" {outDimensions}";
        }
    }
}
