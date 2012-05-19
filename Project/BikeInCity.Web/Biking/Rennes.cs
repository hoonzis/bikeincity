using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Xml.Linq;
using BikeInCity.Model;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Web.Script.Serialization;
using System.Configuration;
using System.Data.SqlClient;
using System.Runtime.Serialization.Json;

namespace BikeInCity.Web.Biking
{
    public class Rennes : IBikeCity
    {
        public List<City> ProcessCity()
        {
            //iformatprovide for the convert method
            IFormatProvider provider = CultureInfo.InvariantCulture.NumberFormat;

            //Getting the RENNES_APP key from the configuration session
            string RENNES_KEY = ConfigurationManager.AppSettings["RENNES_KEY"];

            String rennesUrl = String.Format("http://data.keolis-rennes.com/xml/?version=1.0&key={0}&cmd=getstation", RENNES_KEY);
            XDocument xDoc = XDocument.Load(rennesUrl);
            String cityName = this.GetType().Name;

            var stations = (from c in xDoc.Descendants("opendata").Descendants("answer").Descendants("data").Descendants("station")
                            select new Station
                            {
                                Address = (string)c.Element("name").Value,
                                Id = Convert.ToInt16(c.Element("id").Value, provider),
                                Lat = Convert.ToDouble(c.Element("latitude").Value, provider),
                                Lng = Convert.ToDouble(c.Element("longitude").Value, provider),
                                Free = Convert.ToInt16(c.Element("bikesavailable").Value, provider),
                                Total = Convert.ToInt16(c.Element("bikesavailable").Value, provider) + Convert.ToInt16(c.Element("slotsavailable").Value, provider),
                                //already have some info
                                IsUpdate = true
                            }).ToList();


            City city = new City
            {
                Name = cityName,
                Stations = stations,
                TimeStamp = DateTime.Now
            };

            var cities = new List<City>();
            cities.Add(city);
            return cities;
        }
    }
}