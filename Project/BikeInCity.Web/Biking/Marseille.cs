using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using BikeInCity.Model;

namespace BikeInCity.Web.Biking
{
    public class Marseille : IBikeCity
    {
        public List<City> ProcessCity()
        {
            var city = Decaux.GetCity("http://www.levelo-mpm.fr/", "marseille/");
            
            city.Name = "Marseille";

            var cities = new List<City>();
            cities.Add(city);
            return cities;
        }
    }
}