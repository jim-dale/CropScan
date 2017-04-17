﻿
namespace CropScan
{
    using System;
    using System.Text.RegularExpressions;

    public class ArgProcessor
    {
        private enum State
        {
            ExpectOption,
            ExpectWidth,
            ExpectHeight,
            ExpectSuffix,
            ExpectResolution
        }

        public static AppContext Parse(string[] args)
        {
            var result = new AppContext();

            try
            {
                var state = State.ExpectOption;

                foreach (var arg in args)
                {
                    if (arg == null)
                    {
                        continue;
                    }
                    switch (state)
                    {
                        case State.ExpectWidth:
                            result.WidthCm = TryConvertLength(arg);
                            if (result.WidthCm.HasValue == false)
                            {
                                result.AddFault($"Width parameter not in correct format - \"{arg}\"");
                            }
                            state = State.ExpectOption;
                            break;
                        case State.ExpectHeight:
                            result.HeightCm = TryConvertLength(arg);
                            if (result.HeightCm.HasValue == false)
                            {
                                result.AddFault($"Height parameter not in correct format - \"{arg}\"");
                            }
                            state = State.ExpectOption;
                            break;
                        case State.ExpectSuffix:
                            result.FileSuffix = arg;
                            state = State.ExpectOption;
                            break;
                        case State.ExpectResolution:
                            result.DefaultMinResX = Decimal.Parse(arg);
                            result.DefaultMinResY = Decimal.Parse(arg);
                            state = State.ExpectOption;
                            break;
                        case State.ExpectOption:
                            if (arg.StartsWith("--"))
                            {
                                state = ProcessLongOption(result, arg, state);
                            }
                            else if (arg.StartsWith("-") || arg.StartsWith("/"))
                            {
                                state = ProcessShortOption(result, arg, state);
                            }
                            else
                            {
                                result.SearchPaths.Add(arg);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                result.AddExceptionFault(ex);
            }
            return result;
        }

        private static State ProcessShortOption(AppContext ctx, string arg, State currentState)
        {
            State result = currentState;

            if (arg.Length > 1)
            {
                string option = arg.Substring(1).ToLowerInvariant();

                switch (option)
                {
                    case "?":
                        ctx.ShowHelp = true;
                        break;
                    case "wi":
                        ctx.WhatIf = true;
                        break;
                    case "h":
                        result = State.ExpectHeight;
                        break;
                    case "w":
                        result = State.ExpectWidth;
                        break;
                    case "s":
                        result = State.ExpectSuffix;
                        break;
                    case "r":
                        result = State.ExpectResolution;
                        break;
                    default:
                        ctx.AddInvalidArgFault(arg);
                        break;
                }
            }
            else
            {
                ctx.AddInvalidArgFault(arg);
            }
            return result;
        }

        private static State ProcessLongOption(AppContext ctx, string arg, State currentState)
        {
            State result = currentState;

            if (arg.Length > 2)
            {
                string option = arg.Substring(2).ToLowerInvariant();

                switch (option)
                {
                    case "help":
                        ctx.ShowHelp = true;
                        break;
                    case "whatif":
                        ctx.WhatIf = true;
                        break;
                    case "height":
                        result = State.ExpectHeight;
                        break;
                    case "width":
                        result = State.ExpectWidth;
                        break;
                    case "suffix":
                        result = State.ExpectSuffix;
                        break;
                    case "resolution":
                        result = State.ExpectResolution;
                        break;
                    default:
                        ctx.AddInvalidArgFault(arg);
                        break;
                }
            }
            else
            {
                ctx.AddInvalidArgFault(arg);
            }
            return result;
        }

        public static void ShowHelp()
        {
            Console.WriteLine("Crops an image file given a new height and / or width measurements.");
            Console.WriteLine("The measurements can be specified in centimetres (cm) or inches (in).");
            Console.WriteLine();
            Console.WriteLine("CropScan [-?] [-w width] [-h height] [-s suffix] [-r resolution] [-wi] [drive:][path]filename ...");
            Console.WriteLine();
            Console.WriteLine("  -?             Show this help information.");
            Console.WriteLine("  -w width       New width for the cropped image.");
            Console.WriteLine("  -h height      New height for the cropped image.");
            Console.WriteLine("  -s             Suffix to add to the filename to create a copy of the input file.");
            Console.WriteLine("  -r resolution  Specifies the minimum resolution to assume when resolution information is missing and the");
            Console.WriteLine("                 system defaults to 96dpi. The default value for this setting is 200dpi.");
            Console.WriteLine("  -wi            Displays a message that describes the effect of the command, instead of executing the command.");
            Console.WriteLine("  [drive:][path][filename]");
            Console.WriteLine("                 Specifies drive, directory, and / or files to process.");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("CropScan -h 14.85cm *.jpg");
            Console.WriteLine("CropScan -h 14.85cm -w 10in *.jpg *.png");
            Console.WriteLine();
        }

        private static decimal? TryConvertLength(string input)
        {
            var regexCm = new Regex(Constants.CmPattern, RegexOptions.IgnoreCase);
            var regexIn = new Regex(Constants.InPattern, RegexOptions.IgnoreCase);

            decimal? result = TryConvertLength(regexCm, input, 1);
            if (result == null)
            {
                result = TryConvertLength(regexIn, input, Constants.CmsPerInch);
            }
            return result;
        }

        private static decimal? TryConvertLength(Regex regex, string input, decimal conversionFactor)
        {
            decimal? result = null;

            var match = regex.Match(input);
            if (match.Success)
            {
                decimal value = Decimal.Parse(match.Groups[1].Value);
                value = value * conversionFactor;
                result = Math.Round(value, 2);
            }
            return result;
        }
    }
}