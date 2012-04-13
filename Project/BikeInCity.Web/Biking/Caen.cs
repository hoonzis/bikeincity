using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using BikeInCity.Model;

namespace BikeInCity.Web.Biking
{
    public class Caen : IBikeCity
    {
        public List<City> ProcessCity()
        {
            var city = CityBike.GetCity("http://www.veol.caen.fr/");

            String name = "Caen";
            city.Name = name;

            var cities = new List<City>();
            cities.Add(city);
            return cities;
        }
    }
}