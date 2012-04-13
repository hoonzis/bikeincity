using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BikeInCity.Model;

namespace BikeInCity.Core.Services
{
    public interface IInfoService
    {
        void AddInformationTip(String title, String description, String imageUrl, int cityID,double lat, double lng);
        void UpdateCity(String description, String StationImageUrl, String bikeCityUrl, int cityId);
    }
}
