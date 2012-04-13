using BikeInCity.Web.Biking;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using System.Collections.Generic;

namespace UnitTests
{
    
    
    /// <summary>
    ///This is a test class for PragueTest and is intended
    ///to contain all PragueTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PragueTest
    {
        [TestMethod()]
        public void PragueProcessCityTest()
        {
            Prague target = new Prague();
            var result = target.ProcessCity();
            Assert.AreEqual(result.Count, 1);
        }
    }
}
