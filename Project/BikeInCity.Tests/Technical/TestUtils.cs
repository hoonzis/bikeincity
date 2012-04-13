using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BikeInCity.Model;

namespace UnitTests.Technical
{
    public static class TestUtils
    {
        public static void TestCityResults(List<City> actual)
        {
            Assert.AreEqual(actual.Count, 1);
            Assert.IsTrue(actual[0].Stations.Count > 0);
            Assert.IsTrue(actual[0].Stations[0].Free != -1);
        }
    }
}
