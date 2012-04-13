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
using BikeInCity.Model;
using System.Device.Location;

namespace BikeInCity.ServiceCaller
{
  public class AddressGeocodedEventArgs : EventArgs
  {
    public GeoCoordinate Location {get;set;}
    public State StateType { get; set; }

    public AddressGeocodedEventArgs(GeoCoordinate c, State s)
    {
      this.Location = c;
      this.StateType = s;
    }
  }
}
