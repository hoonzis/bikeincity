using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace BikeInCity.Model
{
  public class Coordinate
  {
    public double Longitude { get; set; }
    public double Latitude { get; set; }

    public Coordinate()
    {

    }

    public Coordinate(double lat, double lng)
    {
      this.Longitude = lng;
      this.Latitude = lat;
    }
  }
}
