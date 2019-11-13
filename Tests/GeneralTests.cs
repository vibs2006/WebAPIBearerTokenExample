using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebAPIBearerTokenExample.Common;

namespace Tests
{
    [TestClass]
    public class GeneralTests
    {
        [TestMethod]
        public void TestFileListings()
        {
            
           
            CustomLogger.DeleteFilesOlderMoreThanNdays(15);

        }
    }
}
