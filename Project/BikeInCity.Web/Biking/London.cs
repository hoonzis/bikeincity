using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Xml.Linq;
using BikeInCity.Model;

namespace BikeInCity.Web.Biking
{
    public class London : IBikeCity
    {
        public List<City> ProcessCity()
        {
            
            //iformatprovide for the convert method
            IFormatProvider provider = CultureInfo.InvariantCulture.NumberFormat;

            XDocument xDoc = XDocument.Load("http://api.bike-stats.co.uk/service/rest/bikestats?format=[xml]");
            var stations = (from c in xDoc.Descendants("dockStationList").Descendants("dockStation")
                            select new Station
                            {
                                Address = (string)c.Element("name").Value,
                                Id = Convert.ToInt16(c.Attribute("ID").Value, provider),
                                Lat = Convert.ToDouble(c.Element("latitude").Value, provider),
                                Lng = Convert.ToDouble(c.Element("longitude").Value, provider),
                                Free = Convert.ToInt16(c.Element("bikesAvailable").Value, provider),
                                Total = Convert.ToInt16(c.Element("bikesAvailable").Value, provider) + Convert.ToInt16(c.Element("emptySlots").Value, provider),
                                //already have some info
                                IsUpdate = true
                            }).ToList();

            //result = (from c in xDoc.Descendants("stations").Descendants("station")
            //          select new Station
            //          {
            //            Address = (string)c.Element("name").Value,
            //            Id = Convert.ToInt16(c.Element("id").Value, provider),
            //            Lat = Convert.ToDouble(c.Element("lat").Value, provider),
            //            Lng = Convert.ToDouble(c.Element("long").Value, provider),
            //            Free = Convert.ToInt16(c.Element("nb_bikes").Value, provider),
            //            Total = Convert.ToInt16(c.Element("nb_bikes").Value, provider) + Convert.ToInt16(c.Element("nb_empty_docks").Value, provider),
            //            //already have some info
            //            IsUpdate = true
            String name = "London";
            City city = new City
            {
                Name = name,
                Stations = stations,
                TimeStamp = DateTime.Now
            };

            var cities = new List<City>();
            cities.Add(city);
            return cities;
        }
    }
}