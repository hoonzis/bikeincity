using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BikeInCity.Model;

namespace BikeInCity.Web.Biking
{
    public interface IBikeCity
    {
        List<City> ProcessCity();
    }
}