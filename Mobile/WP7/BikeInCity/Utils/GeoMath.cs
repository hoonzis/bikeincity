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
using Microsoft.Phone.Controls.Maps.Platform;
using BikeInCity.Model;
using System.Device.Location;

namespace BikeInCity.Utils
{
  public class GeoMath
  {
      public const double R = 6371000;

      /// <summary>
      /// Converts degrees to radians. Used to convert the latitude and longitude coordinates.
      /// </summary>
      /// <param name="angle"></param>
      /// <returns></returns>
      public static double ToRad(double angle)
      {
          var r = angle * Math.PI / 180;
          return r;
      }



      /// <summary>
      /// Returns distsance in meters between two points of earth using the Spherical Law of Cosines
      /// </summary>
      /// <param name="start"></param>
      /// <param name="end"></param>
      /// <returns></returns>
      public static int ComputeDistance(double startLat, double startLng, double endLat, double endLng)
      {
          var lat1 = ToRad(startLat);
          var lng1 = ToRad(startLng);
          var lat2 = ToRad(endLat);
          var lng2 = ToRad(endLng);

          return SphericalEarthProjectedToAPlane(lat1, lng1, lat2, lng2);
      }

      public static int Pythagoras(double lat1, double lng1, double lat2, double lng2)
      {

          double dlng = lng2 - lng1;
          double dlat = lat2 - lat1;

          int d = (int)(R * Math.Sqrt(dlat * dlat + dlng * dlng));
          return d;
      }

      public static int Harvesine(double lat1, double lng1, double lat2, double lng2)
      {
          double dlng = lng2 - lng1;
          double dlat = lat2 - lat1;

          var a = Math.Pow(Math.Sin(dlat / 2), 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(dlng / 2), 2);
          var c = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));

          var d = R * c;

          //harvesine formula

          /* Law of cosines
          var d = Math.Acos(Math.Sin(lat1) * Math.Sin(lat2) +
                            Math.Cos(lat1) * Math.Cos(lat2) *
                            Math.Cos(lng1 - lng2)) * R;
          */
          return (int)d;
      }

      public static int SphericalEarthProjectedToAPlane(double lat1, double lng1, double lat2, double lng2)
      {


          double diflat = lat1 - lat2;
          double diflng = lng1 - lng2;

          double meanLat = (lat1 + lat2) / 2;

          var help = Math.Cos(meanLat) * diflng;
          int d = (int)(R * Math.Sqrt(diflat * diflat + help * help));
          return d;
      }
  }
}
