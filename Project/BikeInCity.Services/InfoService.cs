using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BikeInCity.Core.Services;
using BikeInCity.Model;
using BikeInCity.Core.DataAccess;
using Ninject;
using System.Transactions;

namespace BikeInCity.Services
{
    public class InfoService : IInfoService
    {

        private IRepository _repository;

        [Inject]
        public InfoService(IRepository repository)
        {
            _repository = repository;
        }

        public void AddInformationTip(String title, String descritpion, String imageUrl, int cityID, double lat, double lng)
        {
            using (var scope = new TransactionScope())
            {
                var city = _repository.Load<City>(cityID);
                InformationTip tip = new InformationTip() { Description = descritpion, ImageUrl = imageUrl, Title = title, City = city ,Lat=lat, Lng = lng};
                _repository.Save<InformationTip>(tip);
                city.InformationTips.Add(tip);
                scope.Complete();
            }
        }

        
        public void UpdateCity(string description, string stationImageUrl, string bikeImageUrl, int cityId)
        {
            using (var scope = new TransactionScope())
            {
                City city = _repository.Load<City>(cityId);
                city.Description = description;
                city.StationImageUrl = stationImageUrl;
                city.BikeImageUrl = bikeImageUrl;
                scope.Complete();
            }
        }

    }
}
