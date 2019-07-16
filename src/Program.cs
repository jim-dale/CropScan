using System;
using System.Drawing;
using System.IO;

namespace CropScan
{
    class Program
    {
        static void Main(string[] args)
        {
            var context = new AppContext()
                .UseArgs(args);

            if (context.Faulted == false)
            {
                context.Verify();
            }
            if (context.ShowHelp)
            {
                AppContext.ShowHelpText();
            }
            else if (context.Faulted)
            {
                AppContext.ShowHelpText();
                context.ShowFaults();
            }
            else
            {
                new ForEachFile()
                    .Run(Directory.GetCurrentDirectory(), context.SearchPatterns, string.Empty, (file) =>
                    {
                        CropImageFile(context, file);
                    });
            }
        }

        /// <summary>
        /// Loads an image file, crops it and saves it back to disk.
        /// </summary>
        /// <remarks>
        /// Loads the image into a memory buffer in case the source image file is
        /// overwritten by the cropped image. If the source image is loaded
        /// directly into a Bitmap then the file is locked and can't be overwritten.
        /// There are probably much better ways of cropping an image and retain the source image pixel format
        /// and resolution but this appears to work
        /// </remarks>
        /// <param name="ctx">Encapsulates all app specific information required at runtime</param>
        /// <param name="fileContext">Encapsulates all file specific information required at runtime</param>
        private static void CropImageFile(AppContext ctx, string path)
        {
            using (var stream = new MemoryStream(File.ReadAllBytes(path)))
            {
                using (var sourceImage = new Bitmap(stream))
                {
                    var fileContext = new FileContext { InputPath = path };

                    fileContext.FromSourceImage(sourceImage, ctx.WidthCm, ctx.HeightCm);
                    fileContext.SetOutputFile(fileContext.InputPath, ctx.FileNameSuffix);

                    if (fileContext.SrcResX < ctx.DefaultMinResX || fileContext.SrcResY < ctx.DefaultMinResY)
                    {
                        fileContext.Message = $"\"{fileContext.InputPath}\" {fileContext.SrcResX}x{fileContext.SrcResY} is less than the minimum required resolution";
                    }
                    else
                    {
                        if (fileContext.SrcWidthPx == fileContext.OutWidthPx && fileContext.SrcHeightPx == fileContext.OutHeightPx
                            && fileContext.SrcResX == fileContext.SrcResX && fileContext.SrcResY == fileContext.SrcResY)
                        {
                            fileContext.Message = $"\"{fileContext.InputPath}\" no changes are required";
                        }
                        else
                        {
                            if (ctx.WhatIf == false)
                            {
                                using (var destImage = sourceImage.Clone(fileContext.CropRect, sourceImage.PixelFormat))
                                {
                                    destImage.Save(fileContext.OutputPath);
                                }
                            }
                            fileContext.Message = fileContext.ToString(ctx.WhatIf);
                        }
                    }
                    if (string.IsNullOrEmpty(fileContext.Message) == false)
                    {
                        Console.WriteLine(fileContext.Message);
                    }
                }
            }
        }
    }
}
