using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BikeInCity.Core.Services;
using BikeInCity.Model;
using Ninject;
using BikeInCity.Core.DataAccess;
using System.Xml.Linq;
using System.Globalization;
using System.Net;
using BikeInCity.DataAccess.Repositories;

namespace BikeInCity.Services
{
    public class CityService : ICityService
    {
        public IRepository _repository;

        [Inject]
        public CityService(IRepository repository)
        {
            _repository = repository;
        }

        public void SaveCity(City city)
        {
            _repository.Save<City>(city);
        }

        

        public void UpdateCityStations(City city)
        {

            City cityInDB;
            if (city.Id != 0)
            {
                cityInDB = _repository.Load<City>(city.Id);
            }
            else
            {
                cityInDB = _repository.Find<City>(x => x.Name == city.Name).FirstOrDefault();
            }

            if (cityInDB == null)
            {
                Logger.WriteMessage("City with ID: " + city.Id + " or name " + city.Name + " not found in DB");
                return;
            }
            
            var result = _repository.ExecuteUpdateQuery("delete Station where CityId = " + cityInDB.Id);
            Console.Write(result);

            _repository.Flush();

            _repository.Refresh<City>(cityInDB);

            foreach (var station in city.Stations)
            {
                station.City = cityInDB;
                _repository.Save(station);
            }

            cityInDB.Stations = city.Stations;
            _repository.Update(cityInDB);
            _repository.Flush();
        }

        public void ReinsertCitiesToDB(String nextBikeURL, String cityXMLFile)
        {

            Logger.WriteMessage("NextBike: " + nextBikeURL);
            Logger.WriteMessage("Other: " + cityXMLFile);

            WebClient client = new WebClient();
            var xml = client.DownloadString(nextBikeURL);


            Logger.WriteMessage(xml);

            if (!String.IsNullOrEmpty(xml))
            {
                //Cities from NextBike           
                XDocument xDoc = XDocument.Parse(xml);
                Logger.WriteMessage("nextBike Loaded!");
                foreach (var elem in xDoc.Descendants("city"))
                {
                    var name = elem.Attribute("name").Value;
                    var lat = ToDouble(elem.Attribute("lat").Value);
                    var lng = ToDouble(elem.Attribute("lng").Value);
                    var countryName = FilterCountry(elem.Parent.Attribute("name").Value);

                    CreateIfDoesNotExists(lat, lng, name, countryName);
                }
            }

            //Cities from my defined list
            XDocument xMyCitiesDoc = XDocument.Load(cityXMLFile);
            Logger.WriteMessage("CityFile Loaded");
            foreach (var elem in xMyCitiesDoc.Descendants("city"))
            {
                var countryName = FilterCountry(elem.Attribute("country").Value);
                var lat = Convert.ToDouble(elem.Attribute("lat").Value, CultureInfo.InvariantCulture);
                var lng = Convert.ToDouble(elem.Attribute("lng").Value, CultureInfo.InvariantCulture);
                var name = elem.Attribute("name").Value;

                CreateIfDoesNotExists(lat, lng, name, countryName);
            }
        }

        private void CreateIfDoesNotExists(double lat, double lng, String name, String country){
            var city = _repository.Find<City>(x => x.Name == name).FirstOrDefault();

            if (city == null)
            {
                city = NewCity(country, lat, lng, name);
                _repository.Save<City>(city);
            }
        }

        private City NewCity(string countryName, double lat, double lng, string name)
        {
            var country = _repository.Find<Country>(x => x.Name == countryName).FirstOrDefault();

            if (country == null)
            {
                country = new Country() { Name = countryName };
                _repository.Save<Country>(country);
            }

            City city = new City { Country = country, Lat = lat, Lng = lng, Name = name };
            return city;
        }

        public static double ToDouble(String value)
        {
            try
            {
                return Convert.ToDouble(value, CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {

            }
            return -1;
        }

        public static String FilterCountry(String country)
        {
            country = country.Replace("LEIHRADL nextbike Lower ", String.Empty);
            country = country.Replace("LEIHRADL", String.Empty);
            country = country.Replace("nextbike ", String.Empty);
            country = country.Replace("BalticBike ", String.Empty);
            country = country.Replace("UsedomRad ", String.Empty);
            country = country.Replace("NorisBike ", String.Empty);
            country = country.Replace("metropolradruhr ", String.Empty);
            return country;
        }

        public void ClearDB()
        {
            throw new NotImplementedException();
        }
    }
}
