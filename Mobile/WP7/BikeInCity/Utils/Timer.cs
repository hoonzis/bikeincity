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

namespace BikeInCity.Utils
{
  public static class WP7Timer
  {
    private static DateTime start;
    public static void Start()
    {
      start = DateTime.Now;
    }

    public static void Time(String description)
    {
      TimeSpan t = DateTime.Now.Subtract(start);
      System.Diagnostics.Debug.WriteLine(description + " - " + t.Seconds + ":" +t.Milliseconds);
    }
  }
}
