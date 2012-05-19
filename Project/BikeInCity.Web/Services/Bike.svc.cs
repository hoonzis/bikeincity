using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using BikeInCity.Model;
using System.Web;
using System.Xml.Linq;
using System.Globalization;

using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using BikeInCity.Web.Technical;
using Ninject;
using BikeInCity.Core.Services;
using BikeInCity.Dto;
using BikeInCity.Core.DataAccess;
using System.Net;
using log4net;
using AutoMapper;

namespace BikeInCity.Web.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceContract]
    public class Bike
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(Bike));
        private IRepository _repository;

        public Bike()
        {
            _repository = Global.GetObject<IRepository>();
        }

        [OperationContract]
        [WebGet(UriTemplate = "cities", ResponseFormat = WebMessageFormat.Json)]
        public List<CityDto>  GetAllCities()
        {
            var cities = Mapper.Map<List<CityDto>>(_repository.GetAll<City>());
            return cities;
        }

        [OperationContract]
        [WebGet(UriTemplate = "countries/{countryId}/cities", ResponseFormat = WebMessageFormat.Json)]
        public List<CityDto> GetCities(String countryId)
        {
            try
            {
                int countryID = Convert.ToInt32(countryId);
                var cities = _repository.Find<City>(x => x.Country.Id == countryID);
                return Mapper.Map<List<CityDto>>(cities);
            }
            catch (FormatException ex)
            {
                _log.Error(ex.Message,ex);
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
        }

        [OperationContract]
        [WebGet(UriTemplate = "countries", ResponseFormat = WebMessageFormat.Json)]
        public List<CountryDto> GetCountries()
        {
            var list = _repository.GetAll<Country>();
            var clearedList = list.Select(x => new CountryDto{ Name = x.Name, Id = x.Id }).ToList();
            return clearedList;
        }


        [OperationContract]
        [WebGet(UriTemplate = "city/{cityID}/stations", ResponseFormat = WebMessageFormat.Json)]
        public List<StationDto> GetStations(String cityID)
        {
            int id = Int32.Parse(cityID);
            var stations = _repository.Find<Station>(x => x.City.Id == id);
            var dtos = Mapper.Map<List<StationDto>>(stations);
            return dtos;
        }
    }
}
