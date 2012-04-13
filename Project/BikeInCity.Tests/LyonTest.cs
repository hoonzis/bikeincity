using BikeInCity.Web.Biking;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using System.Collections.Generic;
using UnitTests.Technical;

namespace UnitTests
{
    
    
    /// <summary>
    ///This is a test class for LyonTest and is intended
    ///to contain all LyonTest Unit Tests
    ///</summary>
    [TestClass()]
    public class LyonTest
    {
        /// <summary>
        ///A test for ProcessCity
        ///</summary>
        [TestMethod()]
        public void LyonProcessCityTest()
        {
            Lyon target = new Lyon();
            var result = target.ProcessCity();
            TestUtils.TestCityResults(result);
        }
    }
}
