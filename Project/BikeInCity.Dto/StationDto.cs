using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BikeInCity.Dto
{
  public class StationDto
  {
      public int Id { get; set; }
      public string Address { get; set; }
      public double Lat { get;set; }
      public double Lng { get; set; }
      public int Free{get;set;}
      public int Total { get; set; }
  }
}
