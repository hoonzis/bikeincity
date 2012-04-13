using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BikeInCity.Model;

namespace BikeInCity.Web.Biking
{
    public class HomePort : IBikeCity
    {
        public List<City> ProcessCity()
        {
            BikeInCity.Web.HomePortService.ExternalAPI_122Client _client = new BikeInCity.Web.HomePortService.ExternalAPI_122Client();
            var countries = _client.GetCountries("en");

            return null;
        }
    }
}