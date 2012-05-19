using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BikeInCity.Model;

namespace BikeInCity.Core.Services
{
    public interface ICityService
    {
        void SaveCity(City city);

        void UpdateCityStations(City city);

        void ReinsertCitiesToDB(String nextBikeURL, String xmlCities);

    }
}
