using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using BikeInCity.Model;
using BikeInCity.Web.Technical;

namespace BikeInCity.Web.Biking
{
    public class Prague : IBikeCity
    {
        public List<City> ProcessCity()
        {
            var stations = WebUtils.ProcessManualCity(WebUtils.GetAppDataPath("\\homeport_prague.xml"));

            City city = new City();
            city.Stations = stations;
            city.Name = "Prague";

            var cities = new List<City>();
            cities.Add(city);
            return cities;
        }   
    }
}