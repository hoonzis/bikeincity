using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using BikeInCity.Model;
using BikeInCity.Web.Technical;

namespace BikeInCity.Web.Biking
{
    public class Montreal : IBikeCity
    {

        public List<City> ProcessCity()
        {
            String rennesUrl = String.Format("https://profil.bixi.ca/data/bikeStations.xml");
            XDocument xDoc = XDocument.Load(rennesUrl);

            var stations = (from station in xDoc.Descendants("station")
                            let emptyDocks = Convert.ToInt32(station.Element("nbEmptyDocks").Value)
                            let free = Convert.ToInt32(station.Element("nbBikes").Value)
                            select new Station
                            {
                                Address = station.Element("name").Value,
                                Lat = WebUtils.ToDoubleTolerate(station.Element("lat").Value),
                                Lng = WebUtils.ToDoubleTolerate(station.Element("long").Value),
                                Free = free,
                                Total = emptyDocks + free
                            }).ToList();

            String name = "Montreal";
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