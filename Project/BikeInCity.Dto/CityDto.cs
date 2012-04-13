using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace BikeInCity.Dto
{
    public class CityDto
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public DateTime? TimeStamp { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public int CountryId { get; set; }
    }

}