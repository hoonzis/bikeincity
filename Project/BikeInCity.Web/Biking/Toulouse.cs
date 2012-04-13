using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BikeInCity.Model;

namespace BikeInCity.Web.Biking
{
    public class Toulouse : IBikeCity
    {
        public List<City> ProcessCity()
        {
            var city = Decaux.GetCity("http://www.velo.toulouse.fr/", "toulouse/");
            city.Name = "Toulouse";

            var cities = new List<City>();
            cities.Add(city);
            return cities;
        }
    }
}