using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using System.Collections.Generic;
using BikeInCity.Model;
using UnitTests.Technical;
using BikeInCity.Web.Biking;

namespace UnitTests
{


    [TestClass()]
    public class ToulouseTest
    {
        [TestMethod()]
        public void ToulouseProcessCityTest()
        {
            Toulouse target = new Toulouse();
            List<City> actual = target.ProcessCity();
            TestUtils.TestCityResults(actual);
        }
    }
}
