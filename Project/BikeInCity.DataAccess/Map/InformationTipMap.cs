using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using BikeInCity.Model;

namespace BikeInCity.DataAccess.Map
{
    public class InformationTipMap : ClassMap<InformationTip>
    {
        public InformationTipMap()
        {
            Id(x => x.Id);
            Map(x => x.Title);
            Map(x => x.Description);
            Map(x => x.ImageUrl);
            Map(x => x.Lat);
            Map(x => x.Lng);
            References(x => x.City);
            Map(x => x.Author);
        }
    }
}
