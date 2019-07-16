using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CropScan.UnitTests
{
    [TestClass]
    public class ArgProcessorUnitTests
    {
        [TestMethod]
        public void UseArgs_EmptyArgs_NoFault()
        {
            var ctx = new AppContext()
                .UseArgs(new string[] { });

            Assert.IsFalse(ctx.Faulted);
        }

        [TestMethod]
        public void UseArgs_NullArg_NoFault()
        {
            var ctx = new AppContext()
                .UseArgs(new string[] { null });

            Assert.IsFalse(ctx.Faulted);
        }

        [TestMethod]
        public void UseArgs_EmptyStringArg_NoFault()
        {
            var ctx = new AppContext()
                .UseArgs(new string[] { string.Empty });

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
        public void UseArgs_WidthArgTest(string option, string optionValue, double expectedValue)
        {
            var ctx = new AppContext()
                .UseArgs(new string[] { option, optionValue });

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
        public void UseArgs_HeightArgTest(string option, string optionValue, double expectedValue)
        {
            var ctx = new AppContext()
                .UseArgs(new string[] { option, optionValue });

            Assert.IsFalse(ctx.Faulted);
            Assert.AreEqual((decimal)expectedValue, ctx.HeightCm);
        }

        [TestMethod]
        [DataRow("*.jpg;*.png", "*.jpg;*.png")]
        public void UseArgs_SearchPatternsArgTests(string optionValue, string expectedValue)
        {
            var ctx = new AppContext()
                .UseArgs(new string[] { optionValue });

            Assert.IsFalse(ctx.Faulted);
            Assert.AreEqual(expectedValue, ctx.SearchPatterns);
        }

        [DataTestMethod]
        [DataRow("-s", "", "")]
        [DataRow("-s", "-cropped", "-cropped")]
        [DataRow("--suffix", "-cropped", "-cropped")]
        public void UseArgs_SuffixArgTests(string option, string optionValue, string expectedValue)
        {
            var ctx = new AppContext()
                .UseArgs(new string[] { option, optionValue });

            Assert.IsFalse(ctx.Faulted);
            Assert.AreEqual(expectedValue, ctx.FileNameSuffix);
        }

        [DataTestMethod]
        [DataRow("-wi")]
        [DataRow("-WI")]
        [DataRow("--whatif")]
        [DataRow("--WhatIf")]
        public void UseArgs_WhatIfArgTests(string option)
        {
            var ctx = new AppContext()
                .UseArgs(new string[] { option });

            Assert.IsFalse(ctx.Faulted);
            Assert.IsTrue(ctx.WhatIf);
        }

        [TestMethod]
        public void UseArgs_MultipleArgsTests()
        {
            string suffixValue = "-cropped";
            string searchPatterns = "*.jpg";
            decimal expectedHeigth = 14.85M;
            decimal expectedWidth = 11.58M;

            var ctx = new AppContext()
                .UseArgs(new string[] { "-s", suffixValue, "-w", "4.56in", "-h", "14.85 cm", searchPatterns });

            Assert.IsFalse(ctx.Faulted);
            Assert.IsFalse(ctx.WhatIf);
            Assert.AreEqual(suffixValue, ctx.FileNameSuffix);
            Assert.AreEqual(searchPatterns, ctx.SearchPatterns);
            Assert.AreEqual(expectedHeigth, ctx.HeightCm.Value);
            Assert.AreEqual(expectedWidth, ctx.WidthCm.Value);
        }
    }
}
