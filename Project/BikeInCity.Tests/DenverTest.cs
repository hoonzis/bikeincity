using BikeInCity.Web.Biking;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using System.Collections.Generic;

namespace UnitTests
{
    
    
    /// <summary>
    ///This is a test class for DenverTest and is intended
    ///to contain all DenverTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DenverTest
    {

        [TestMethod()]
        public void DenverProcessCityTest()
        {
            Denver target = new Denver();
            var result = target.ProcessCity();
            Assert.AreEqual(result.Count, 1);
        }
    }
}
