using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BikeInCity.Web.Biking;
using UnitTests.Technical;
using BikeInCity.Model;

namespace UnitTests
{
    [TestClass]
    public class CitiesTests
    {
        [TestMethod]
        public void TestMethod1()
        {
        }

        [TestMethod()]
        [Ignore]
        public void DenverProcessCityTest()
        {
            Denver target = new Denver();
            var result = target.ProcessCity();
            Assert.AreEqual(result.Count, 1);
        }

        [Ignore]
        [TestMethod()]
        public void LyonProcessCityTest()
        {
            Lyon target = new Lyon();
            var result = target.ProcessCity();
            TestUtils.TestCityResults(result);
        }

        [TestMethod()]
        [Ignore]
        public void MarseilleProcessCityTest()
        {
            Marseille target = new Marseille();
            List<City> actual = target.ProcessCity();
            TestUtils.TestCityResults(actual);
        }

        [TestMethod()]
        [Ignore]
        public void PragueProcessCityTest()
        {
            Prague target = new Prague();
            var result = target.ProcessCity();
            Assert.AreEqual(result.Count, 1);
        }

        [TestMethod()]
        [Ignore]
        public void ToulouseProcessCityTest()
        {
            Toulouse target = new Toulouse();
            List<City> actual = target.ProcessCity();
            TestUtils.TestCityResults(actual);
        }

        [TestMethod()]
        [Ignore]
        public void ValenciaProcessCityTest()
        {
            Valencia target = new Valencia();
            List<City> actual = target.ProcessCity();
            Assert.AreEqual(actual.Count, 1);
            Assert.IsTrue(actual[0].Stations.Count > 0);
            Assert.IsTrue(actual[0].Stations[0].Lat != 0);
            Assert.IsTrue(actual[0].Stations[0].Free != -1);
        }
        
        [TestMethod()]
        [Ignore]
        public void WienProcessCityTest()
        {
            Wien target = new Wien();
            var result = target.ProcessCity();
            Assert.AreEqual(result.Count, 1);
        }

        [TestMethod]
        [Ignore]
        public void TestRennes()
        {
            Rennes target = new Rennes();
            var result = target.ProcessCity();
            Assert.AreEqual(result.Count, 1);
            TestUtils.TestCityResults(result);
        }
    }
}
