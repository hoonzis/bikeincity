using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BikeInCity.Web.Technical;
using BikeInCity.Model;

namespace BikeInCity.Web.Biking
{
    public class Brno : IBikeCity
    {
        public List<City> ProcessCity()
        {
            Random r = new Random();
            var stations = WebUtils.ProcessManualCity(WebUtils.GetAppDataPath("brno_test.xml"));

            foreach (var station in stations)
            {
                station.Free = r.Next(10);
                station.Total = 10;
            }

            City city = new City
            {
                Stations = stations
            };

            city.Name = "Brno";

            var cities = new List<City>();
            cities.Add(city);
            return cities;

        }
    }
}