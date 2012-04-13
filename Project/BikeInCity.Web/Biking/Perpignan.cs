using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BikeInCity.Model;

namespace BikeInCity.Web.Biking
{
    public class Perpignan : IBikeCity
    {
        public List<City> ProcessCity()
        {
            var city = CityBike.GetCity("http://www.bip-perpignan.fr/");
            city.Name = "Perpignan";
            
            var cities = new List<City>();
            cities.Add(city);
            return cities;
        }
    }
}