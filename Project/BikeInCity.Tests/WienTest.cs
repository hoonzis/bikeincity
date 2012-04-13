using BikeInCity.Web.Biking;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using System.Collections.Generic;

namespace UnitTests
{
    
    
    /// <summary>
    ///This is a test class for WienTest and is intended
    ///to contain all WienTest Unit Tests
    ///</summary>
    [TestClass()]
    public class WienTest
    {

        [TestMethod()]
        public void WienProcessCityTest()
        {
            Wien target = new Wien();
            var result = target.ProcessCity();
            Assert.AreEqual(result.Count, 1);
        }
    }
}
