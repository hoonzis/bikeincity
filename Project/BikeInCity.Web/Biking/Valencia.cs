using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using BikeInCity.Model;

namespace BikeInCity.Web.Biking
{
    public class Valencia : IBikeCity
    {

        public List<City> ProcessCity()
        {
            var city = Decaux.GetCity("http://www.valenbisi.es/", "valence/");
            city.Name = "Valencia";
            
            var cities = new List<City>();
            cities.Add(city);
            return cities;
        }
    }
}