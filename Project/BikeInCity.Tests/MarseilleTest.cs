using BikeInCity.Web.Biking;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using System.Collections.Generic;
using BikeInCity.Model;
using UnitTests.Technical;

namespace UnitTests
{


    [TestClass()]
    public class MarseilleTest
    {

        [TestMethod()]
        public void MarseilleProcessCityTest()
        {
            Marseille target = new Marseille();
            List<City> actual = target.ProcessCity();
            TestUtils.TestCityResults(actual);
        }
    }
}
