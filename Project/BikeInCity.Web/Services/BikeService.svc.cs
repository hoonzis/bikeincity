using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using BikeInCity.Model;
using BikeInCity.Dto;
using BikeInCity.Core.DataAccess;

namespace BikeInCity.Web.Services
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class BikeService
    {
        private IRepository _repository;

        public BikeService()
        {
            _repository = Global.GetObject<IRepository>();
        }

        [OperationContract]
        [WebGet(UriTemplate = "city?id={stringID}", ResponseFormat = WebMessageFormat.Json)]
        public CityOldDto BikeStations(String stringID)
        {
            int id = Int32.Parse(stringID);
            var city = _repository.Get<City>(id);
            var cityDto = Mapper.MapOldCity(city);

            var stations = _repository.Find<Station>(x => x.City.Id == id);
            var cleared = stations.Select(x => Mapper.MapOldStation(x));
            cityDto.Stations = cleared.ToList();
            return cityDto;
        }

        [OperationContract]
        [WebGet(UriTemplate = "cities", ResponseFormat = WebMessageFormat.Json)]
        public List<CityOldDto> GetCities()
        {
            return _repository.GetAll<City>().Select(x => Mapper.MapOldCity(x)).ToList();
        }

        [OperationContract]
        [WebGet(UriTemplate = "countries", ResponseFormat = WebMessageFormat.Json)]
        public List<String> GetCountries()
        {
            var list = _repository.GetAll<Country>();
            var clearedList = list.Select(x => x.Name).ToList();
            return clearedList;
        }
    }
}
