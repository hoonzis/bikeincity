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
using System.Collections.Generic;

namespace BikeInCity.Model
{
    public class City
    {
        public List<Station> Stations { get; set; }
        public List<Station> FromStations { get; set; }
        public Station CurrentStation { get; set; }

        public String Name { get; set; }
        public int Id { get; set; }
        public DateTime? TimeStamp { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }

        //public double CenterLat { get; set; }
        //public double CenterLng { get; set; }
        public double Zoom { get; set; }

    }
}
