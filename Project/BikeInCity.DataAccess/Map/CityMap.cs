using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using BikeInCity.Model;

namespace BikeInCity.DataAccess.Map
{
    public class CityMap : ClassMap<City>
    {
        public CityMap()
        {
            Id(x => x.Id);
            Map(x => x.Lat);
            Map(x => x.Lng);
            Map(x => x.TimeStamp);
            //Map(x => x.State);
            Map(x => x.Name).Index("city_name");
            Map(x => x.Description);
            Map(x => x.StationImageUrl);
            Map(x => x.BikeImageUrl);
            HasMany(x => x.Stations);
            HasMany(x => x.InformationTips);
            References(x => x.Country);
        }
    }
}
