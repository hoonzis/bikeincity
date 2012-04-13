using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Globalization;
using BikeInCity.Model;

namespace BikeInCity.Web.Biking
{
    public class NextBike : IBikeCity
    {
        //iformatprovide for the convert method
        IFormatProvider provider = CultureInfo.InvariantCulture.NumberFormat;

        public List<City> ProcessCity()
        {
            
            XDocument xDoc = XDocument.Load("http://nextbike.net/maps/nextbike-official.xml");
            var cities = from city in xDoc.Descendants("city")
                         let stations = from station in city.Elements("place")
                                        select new Station
                                        {
                                            Lat = Double.Parse(station.Attribute("lat").Value, provider),
                                            Lng = Double.Parse(station.Attribute("lng").Value, provider),
                                            Address = station.Attribute("name").Value,
                                            Free = SpecialConvert(station.Attribute("bikes").Value),
                                            Total = -1
                                        }
                         select new City
                         {
                             Name = city.Attribute("name").Value,
                             Stations = stations.ToList(),
                             TimeStamp = DateTime.Now
                         };

            
            return cities.ToList();
        }

        private int SpecialConvert(String freeValue)
        {
            if (freeValue == "5+")
            {
                return 5;
            }
            else
            {
                int result = 0;
                Int32.TryParse(freeValue, out result);
                return result;
            }
        }
    }
}