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

        /// <summary>
        /// Reinserts in to the database the cities from different sources
        /// </summary>
        /// <param name="nextBikeURL">Url of the NextBike cities file</param>
        /// <param name="xmlCities">Path to the file containg other cities</param>
        void ReinsertCitiesToDB(String nextBikeURL, String xmlCities);



    }
}
