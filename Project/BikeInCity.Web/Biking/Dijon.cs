using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BikeInCity.Model;

namespace BikeInCity.Web.Biking
{
    public class Dijon : IBikeCity
    {
        public List<City> ProcessCity()
        {
            var city = CityBike.GetCity("http://www.velodi.net/");
            
            city.Name = "Dijon";

            var cities = new List<City>();
            cities.Add(city);
            return cities;
        }
    }
}