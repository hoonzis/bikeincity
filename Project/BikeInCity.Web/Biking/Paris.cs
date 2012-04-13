using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using BikeInCity.Model;
using System.Net;

namespace BikeInCity.Web.Biking
{
    public class Paris : IBikeCity
    {
        public List<City> ProcessCity()
        {            
            var city = Decaux.GetCity("http://www.velib.paris.fr/", String.Empty);
            
            city.Name = "Paris";

            var cities = new List<City>();
            cities.Add(city);
            return cities;
        }
    }
}