using BikeInCity.Web.Biking;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using System.Collections.Generic;
using BikeInCity.Model;

namespace UnitTests
{
    [TestClass()]
    public class ValenciaTest
    {
        [TestMethod()]
        public void ValenciaProcessCityTest()
        {
            Valencia target = new Valencia();
            List<City> actual = target.ProcessCity();
            Assert.AreEqual(actual.Count, 1);
            Assert.IsTrue(actual[0].Stations.Count > 0);
            Assert.IsTrue(actual[0].Stations[0].Lat != 0);
            Assert.IsTrue(actual[0].Stations[0].Free != -1);
        }
    }
}
