
namespace CropScan
{
    public static class Constants
    {
        public const decimal CmsPerInch = 2.54M;
        public const decimal DefaultMinResolution = 100;
        public const decimal MinAllowedResolution = 72;

        public const string StartOfString = "^";
        public const string EndOfString = "$";
        public const string DecimalDigits = @"((?:\d*\.)?\d+)";
        public const string OptionalWhitespace = @"\s*";
        public const string OptionalUnitsCm = "(cm|centimetre|centimetres|centimeter|centimeters)?";
        public const string OptionalUnitsIn = "(in|inch|inches)";

        public const string CmPattern = StartOfString + DecimalDigits + OptionalWhitespace + OptionalUnitsCm + EndOfString;
        public const string InPattern = StartOfString + DecimalDigits + OptionalWhitespace + OptionalUnitsIn + EndOfString;
    }
}
