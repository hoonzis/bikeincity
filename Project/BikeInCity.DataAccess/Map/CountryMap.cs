using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BikeInCity.Model;
using FluentNHibernate.Mapping;

namespace BikeInCity.DataAccess.Map
{
    public class CountryMap : ClassMap<Country>
    {
        public CountryMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            HasMany(x => x.Cities);
        }
    }
}
