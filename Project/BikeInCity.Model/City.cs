using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace BikeInCity.Model
{
    public class City
    {
        public virtual int Id { get; set; }
        public virtual String Name { get; set; }
        public virtual DateTime? TimeStamp { get; set; }
        //public virtual String State { get; set; }
        public virtual double Lat { get; set; }
        public virtual double Lng { get; set; }
        public virtual Country Country { get; set; }
        public virtual String Description { get; set; }
        public virtual String StationImageUrl { get; set; }
        public virtual String BikeImageUrl { get; set; }
        public virtual IList<Station> Stations { get; set; }
        public virtual IList<InformationTip> InformationTips { get; set; }
    }
}