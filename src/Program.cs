using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace CropScan
{
    class Program
    {
        static void Main(string[] args)
        {
            AppContext ctx = ArgProcessor.Parse(args);
            if (ctx.Faulted == false)
            {
                ctx.Verify();
            }
            if (ctx.ShowHelp)
            {
                ArgProcessor.ShowHelp();
            }
            else if (ctx.Faulted)
            {
                ctx.ShowFaults();
            }
            else
            {
                GetMatchingFiles(ctx.SearchPaths, ctx.Files);
                if (ctx.Files.Count == 0)
                {
                    Console.WriteLine("No matching files.");
                }
                else
                {
                    foreach (var sourcePath in ctx.Files)
                    {
                        CropImageFile(ctx, sourcePath);
                    }
                }
            }
        }

        /// <summary>
        /// Loads an image file, crops it and finally saves it back to disk.
        /// </summary>
        /// <remarks>
        /// Loads the image into a memory buffer in case the source image file is
        /// overwritten by the cropped image. If the source image is loaded
        /// directly into a Bitmap then the file is essentially locked and
        /// can't be overwritten.
        /// </remarks>
        /// <param name="ctx">Encapsulates all app specific information required at runtime.</param>
        /// <param name="path">Path to the source image file.</param>
        private static void CropImageFile(AppContext ctx, string path)
        {
            using (var stream = new MemoryStream(File.ReadAllBytes(path)))
            {
                using (var srcImage = new Bitmap(stream))
                {
                    decimal resX = Math.Round((decimal)srcImage.HorizontalResolution, 2);
                    decimal resY = Math.Round((decimal)srcImage.VerticalResolution, 2);

                    resX = Math.Max(resX, ctx.DefaultMinResX);
                    resY = Math.Max(resY, ctx.DefaultMinResY);

                    var cropCtx = new CropContext
                    {
                        SourcePath = path,
                        DestinationPath = GetNewFileName(path, ctx.FileSuffix),
                        SrcWidthPx = srcImage.Width,
                        SrcHeightPx = srcImage.Height,
                        SrcWidthCm = ConvertPixelsToCm(srcImage.Width, resX),
                        SrcHeightCm = ConvertPixelsToCm(srcImage.Height, resY),
                        DestWidthPx = CalculateDimension(srcImage.Width, resX, ctx.WidthCm),
                        DestHeightPx = CalculateDimension(srcImage.Height, resY, ctx.HeightCm),
                        DestWidthCm = ctx.WidthCm,
                        DestHeightCm = ctx.HeightCm,
                        ResX = resX,
                        ResY = resY
                    };

                    var cropRect = new Rectangle(0, 0, cropCtx.DestWidthPx, cropCtx.DestHeightPx);

                    if (ctx.WhatIf == false)
                    {
                        using (var destImage = srcImage.Clone(cropRect, srcImage.PixelFormat))
                        {
                            destImage.Save(cropCtx.DestinationPath);
                        }
                    }
                    cropCtx.Message = GetCropDescription(cropCtx, ctx.WhatIf);
                    Console.WriteLine(cropCtx.Message);
                }
            }
        }

        private static int CalculateDimension(int sourcePixels, decimal resolution, decimal? targetCm)
        {
            int result = sourcePixels;

            if (targetCm.HasValue)
            {
                decimal destPixels = (targetCm.Value / Constants.CmsPerInch) * resolution;
                result = (int)Math.Round(destPixels);
                result = Math.Min(sourcePixels, result);
            }
            return result;
        }

        private static decimal ConvertPixelsToCm(int pixels, decimal resolution)
        {
            decimal result = (pixels / resolution) * Constants.CmsPerInch;
            return Math.Round(result, 2);
        }

        private static string GetNewFileName(string path, string suffix)
        {
            string result = path;
            if (string.IsNullOrEmpty(suffix) == false)
            {
                string directory = Path.GetDirectoryName(path);
                string name = Path.GetFileNameWithoutExtension(path);
                string ext = Path.GetExtension(path);

                name = name + suffix + ext;
                result = Path.Combine(directory, name);
            }
            return result;
        }

        private static void GetMatchingFiles(IReadOnlyCollection<string> searchPaths, ICollection<string> files)
        {
            foreach (var searchPath in searchPaths)
            {
                GetMatchingFiles(searchPath, files);
            }
        }

        private static void GetMatchingFiles(string path, ICollection<string> files)
        {
            var request = Utility.GetDirectorySearchRequest(path);

            var tempFiles = Directory.EnumerateFiles(request.Directory, request.SearchPattern);
            foreach (string file in tempFiles)
            {
                files.Add(file);
            }
        }

        private static string GetCropDescription(CropContext ctx, bool whatIf)
        {
            string prefix = (whatIf) ? "WhatIf: " : String.Empty;
            string srcDimensions = $"{ctx.SrcWidthCm}x" + $"{ctx.SrcHeightCm}cm (WxH) ";
            decimal destWidthCm = ctx.DestWidthCm ?? ctx.SrcWidthCm;
            decimal destHeightCm = ctx.DestHeightCm ?? ctx.SrcHeightCm;
            string destDimensions = $"{destWidthCm}x" + $"{destHeightCm}cm (WxH) ";

            string result = prefix + $"\"{ctx.SourcePath}\" {srcDimensions} => \"{ctx.DestinationPath}\" {destDimensions}";
            return result;
        }
    }
}
