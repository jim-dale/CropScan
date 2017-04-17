using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CropScan.UnitTests
{
    [TestClass]
    public class ArgProcessorUnitTests
    {
        string[] DefaultOptions = { "-h", "10cm" };

        [TestMethod]
        public void EmptyArgListTest()
        {
            var ctx = ArgProcessor.Parse(new string[] { });

            Assert.IsNotNull(ctx);
            Assert.IsFalse(ctx.Faulted);
        }

        [TestMethod]
        public void NullArgTest()
        {
            var ctx = ArgProcessor.Parse(new string[] { null });

            Assert.IsNotNull(ctx);
            Assert.IsFalse(ctx.Faulted);
        }

        [TestMethod]
        public void EmptyArgTest()
        {
            var ctx = ArgProcessor.Parse(new string[] { String.Empty });

            Assert.IsNotNull(ctx);
            Assert.IsFalse(ctx.Faulted);
        }

        [DataTestMethod]
        [DataRow("-w", "14.85", 14.85)]
        [DataRow("-w", "14.85cm", 14.85)]
        [DataRow("-w", "14.85 cm", 14.85)]
        [DataRow("-w", "14.85  cm", 14.85)]
        [DataRow("-w", "14.85 centimetre", 14.85)]
        [DataRow("-w", "14.85 centimeter", 14.85)]
        [DataRow("-w", "14.85 centimetres", 14.85)]
        [DataRow("-w", "14.85 centimeters", 14.85)]
        [DataRow("-w", "4.5 in", 11.43)]
        [DataRow("-w", "1.0 inch", 2.54)]
        [DataRow("-w", "5 inches", 12.7)]
        [DataRow("--width", "2.85in", 7.24)]
        public void WidthArgTest(string option, string optionValue, double expectedValue)
        {
            var ctx = ArgProcessor.Parse(new string[] { option, optionValue });

            Assert.IsNotNull(ctx);
            Assert.IsFalse(ctx.Faulted);
            Assert.AreEqual((decimal)expectedValue, ctx.WidthCm);
        }

        [DataTestMethod]
        [DataRow("-h", "14.85", 14.85)]
        [DataRow("-h", "14.85cm", 14.85)]
        [DataRow("-h", "14.85 cm", 14.85)]
        [DataRow("-h", "14.85  cm", 14.85)]
        [DataRow("-h", "14.85 centimetre", 14.85)]
        [DataRow("-h", "14.85 centimeter", 14.85)]
        [DataRow("-h", "14.85 centimetres", 14.85)]
        [DataRow("-h", "14.85 centimeters", 14.85)]
        [DataRow("-h", "4.5 in", 11.43)]
        [DataRow("-h", "1.0 inch", 2.54)]
        [DataRow("-h", "5 inches", 12.70)]
        [DataRow("--height", "2.6in", 6.60)]
        public void HeightArgTest(string option, string optionValue, double expectedValue)
        {
            var ctx = ArgProcessor.Parse(new string[] { option, optionValue });

            Assert.IsNotNull(ctx);
            Assert.IsFalse(ctx.Faulted);
            Assert.AreEqual((decimal)expectedValue, ctx.HeightCm);
        }

        [TestMethod]
        public void SearchPathsArgTest()
        {
            string[] args = { "*.jpg", @"C:\Users\Public\Pictures\*.png" };

            var ctx = ArgProcessor.Parse(args);

            Assert.IsNotNull(ctx);
            Assert.IsFalse(ctx.Faulted);
            CollectionAssert.AreEquivalent(ctx.SearchPaths, args);
        }

        [DataTestMethod]
        [DataRow("-s", "", "")]
        [DataRow("-s", "-cropped", "-cropped")]
        [DataRow("--suffix", "-cropped", "-cropped")]
        public void SuffixArgTest(string option, string optionValue, string expectedValue)
        {
            var ctx = ArgProcessor.Parse(new string[] { option, optionValue });

            Assert.IsNotNull(ctx);
            Assert.IsFalse(ctx.Faulted);
            Assert.AreEqual(expectedValue, ctx.FileSuffix);
        }

        [DataTestMethod]
        [DataRow("-r", "200.76", 200.76)]
        [DataRow("-R", "300", 300.0)]
        [DataRow("--resolution", "600", 600)]
        [DataRow("--ResoLutioN", "150", 150)]
        public void ResolutionArgTest(string option, string optionValue, double expectedValue)
        {
            string[] options = MergeOptions(DefaultOptions, new string[] { option, optionValue });

            var ctx = ArgProcessor.Parse(options);
            ctx.Verify();

            Assert.IsNotNull(ctx);
            Assert.IsFalse(ctx.Faulted);
            Assert.AreEqual((decimal)expectedValue, ctx.DefaultMinResX);
            Assert.AreEqual((decimal)expectedValue, ctx.DefaultMinResY);
        }

        [DataTestMethod]
        [DataRow("-r", "")]
        [DataRow("--resolution", "0")]
        public void ResolutionInvalidArgTest(string option, string optionValue)
        {
            string[] options = MergeOptions(DefaultOptions, new string[] { option, optionValue });

            var ctx = ArgProcessor.Parse(new string[] { option, optionValue });
            ctx.Verify();

            Assert.IsNotNull(ctx);
            Assert.IsTrue(ctx.Faulted);
        }

        [DataTestMethod]
        [DataRow("-wi")]
        [DataRow("-WI")]
        [DataRow("--whatif")]
        [DataRow("--WhatIf")]
        public void WhatIfArgTest(string option)
        {
            var ctx = ArgProcessor.Parse(new string[] { option });

            Assert.IsNotNull(ctx);
            Assert.IsFalse(ctx.Faulted);
            Assert.IsTrue(ctx.WhatIf);
        }

        [TestMethod]
        public void CommandLineTest()
        {
            string suffixValue = "-cropped";
            string searchPattern = "*.jpg";
            decimal expectedHeigth = 14.85M;
            decimal expectedWidth = 11.58M;

            var ctx = ArgProcessor.Parse(new string[] { "-s", suffixValue, "-w", "4.56in", "-h", "14.85 cm", searchPattern });

            Assert.IsNotNull(ctx);
            Assert.IsFalse(ctx.Faulted);
            Assert.IsFalse(ctx.WhatIf);
            Assert.AreEqual(suffixValue, ctx.FileSuffix);
            Assert.AreEqual(searchPattern, ctx.SearchPaths[0]);
            Assert.AreEqual(expectedHeigth, ctx.HeightCm.Value);
            Assert.AreEqual(expectedWidth, ctx.WidthCm.Value);
        }

        static string[] MergeOptions(string[] options1, string[] options2)
        {
            return options1.Concat(options2).ToArray();
        }
    }
}
