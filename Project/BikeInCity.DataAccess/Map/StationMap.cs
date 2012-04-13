using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using BikeInCity.Model;

namespace BikeInCity.DataAccess.Map
{
    public class StationMap : ClassMap<Station>
    {
        public StationMap()
        {
            Id(x => x.Id);
            Map(x => x.Free);
            Map(x => x.Lat);
            Map(x => x.Lng);
            Map(x => x.Ticket);
            Map(x => x.Total);
            Map(x => x.Address);
            References(x => x.City).Column("CityId");
        }
    }
}
