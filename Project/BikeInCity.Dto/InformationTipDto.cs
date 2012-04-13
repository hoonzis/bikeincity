using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BikeInCity.Dto
{
    public class InformationTipDto
    {
        public int CityId { get; set; }
        public String Title {get;set;}
        public String Description { get; set; }
        public String ImageUrl { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
    }
}
