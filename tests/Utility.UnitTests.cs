using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CropScan.UnitTests
{
    [TestClass]
    public class UtilityUnitTests
    {
        [TestMethod]
        public void EmptyPathTest()
        {
            var actual = Utility.GetDirectorySearchRequest(String.Empty);

            Assert.AreEqual(Directory.GetCurrentDirectory(), actual.Directory);
            Assert.AreEqual("*", actual.SearchPattern);
        }

        [TestMethod]
        public void JustDirectoryTest()
        {
            var actual = Utility.GetDirectorySearchRequest(Environment.SystemDirectory);

            Assert.AreEqual(Environment.SystemDirectory, actual.Directory);
            Assert.AreEqual("*", actual.SearchPattern);
        }

        [TestMethod]
        public void JustSearchPatternTest()
        {
            var actual = Utility.GetDirectorySearchRequest("*.png");

            Assert.AreEqual(Directory.GetCurrentDirectory(), actual.Directory);
            Assert.AreEqual("*.png", actual.SearchPattern);
        }

        [TestMethod]
        public void CurrentDirectoryAndSearchPatternTest()
        {
            var actual = Utility.GetDirectorySearchRequest(@".\*.png");

            Assert.AreEqual(".", actual.Directory);
            Assert.AreEqual("*.png", actual.SearchPattern);
        }

        [TestMethod]
        public void FullDirectoryPathAndSearchPatternTest()
        {
            var actual = Utility.GetDirectorySearchRequest(@"C:\Users\Public\Pictures\*.png");

            Assert.AreEqual(@"C:\Users\Public\Pictures", actual.Directory);
            Assert.AreEqual("*.png", actual.SearchPattern);
        }
    }
}
