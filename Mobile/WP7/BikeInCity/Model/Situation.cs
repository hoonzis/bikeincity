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
    public class Situation
    {
        public double FromLat { get; set; }
        public double ToLat { get; set; }
        public double FromLng { get; set; }
        public double ToLng { get; set; }

        public City CurrentCity { get; set; }
        public Station CurrentStastion { get; set; }


    }
}
