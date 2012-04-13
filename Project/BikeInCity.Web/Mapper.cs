using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BikeInCity.Dto;
using BikeInCity.Model;

namespace BikeInCity.Web
{
    public class Mapper
    {
        public static CityDto Map(City x)
        {
            return new CityDto { Id = x.Id, Lat = x.Lat, Lng = x.Lng, Name = x.Name, TimeStamp = x.TimeStamp, CountryId = x.Country.Id };
        }

        public static InformationTipDto Map(InformationTip x) {
            return new InformationTipDto {CityId = x.City.Id,Description = x.Description, ImageUrl = x.ImageUrl,Lat = x.Lat, Lng = x.Lng, Title =x.Title};
        }

        public static StationDto Map(Station x)
        {
            return new StationDto { Address = x.Address, Free = x.Free, Id = x.Id, Lat = x.Lat, Lng = x.Lng, Ticket = x.Ticket, Total = x.Total };
        }
    }
}