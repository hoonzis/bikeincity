using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BikeInCity.Dto
{
    public class CityOldDto
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public List<StationOldDto> Stations { get; set; }
        public DateTime? TimeStamp { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public String Country { get; set; }
    }
}
