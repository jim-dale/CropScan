
namespace CropScan
{
    using System;

    public static partial class Utility
    {
        public static int CalculateDimension(int sourcePixels, decimal resolution, decimal? outputCms)
        {
            int result = sourcePixels;

            if (outputCms.HasValue)
            {
                result = ConvertCmsToPixels(outputCms.Value, resolution);

                result = Math.Min(sourcePixels, result);
            }
            return result;
        }

        public static decimal ConvertPixelsToCms(int value, decimal resolution)
        {
            decimal result = (value * Constants.CmsPerInch) / resolution;

            return Math.Round(result, 2);
        }

        public static int ConvertCmsToPixels(decimal value, decimal resolution)
        {
            decimal result = (value * resolution) / Constants.CmsPerInch;

            return (int)Math.Round(result);
        }
    }
}
