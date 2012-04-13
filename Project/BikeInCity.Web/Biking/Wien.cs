using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using BikeInCity.Model;
using BikeInCity.Web.Technical;

namespace BikeInCity.Web.Biking
{
    public class Wien : IBikeCity
    {

        public List<City> ProcessCity()
        {
            String url = String.Format("http://dynamisch.citybikewien.at/citybike_xml.php");
            XDocument xDoc = XDocument.Load(url);

            var stations = (from station in xDoc.Descendants("station")
                            let free = Convert.ToInt32(station.Element("free_bikes").Value)
                            let freeboxes = Convert.ToInt32(station.Element("free_boxes").Value)
                            select new Station
                            {
                                Address = station.Element("name").Value,
                                Lat =  WebUtils.ToDoubleTolerate(station.Element("latitude").Value),
                                Lng = WebUtils.ToDoubleTolerate(station.Element("longitude").Value),
                                Free = free,
                                Total = free + freeboxes
                            }).ToList();

            String name = "Wien";
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