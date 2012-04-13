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
using System.Windows.Data;
using System.Globalization;
using Microsoft.Phone.Controls.Maps;
using BikeInCity.Model;
using System.Collections.Generic;
using System.Device.Location;

namespace BikeInCity.Utils
{
  public class LocationsConverter :IValueConverter
  {
    /// <summary>
    /// Converts a value.
    /// </summary>
    /// <param name="value">The value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>
    /// A converted value. If the method returns null, the valid null value is used.
    /// </returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      List<Coordinate> coordinates = value as List<Coordinate>;
      LocationCollection collection = new LocationCollection();
      foreach (Coordinate coord in coordinates)
      {
        collection.Add(new GeoCoordinate(coord.Latitude,coord.Longitude));
      }
      return collection;
    }

    /// <summary>
    /// Converts a value.
    /// </summary>
    /// <param name="value">The value that is produced by the binding target.</param>
    /// <param name="targetType">The type to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>
    /// A converted value. If the method returns null, the valid null value is used.
    /// </returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var collection = (LocationCollection)value;
      List<Coordinate> coordinates = new List<Coordinate>(collection.Count);
      foreach(GeoCoordinate coord in collection){
        coordinates.Add(new Coordinate(coord.Latitude,coord.Longitude));
      }
      return coordinates;
    }
  }
}
