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
    public class Station
    {
        public bool IsUpdate
        {
            get;
            set;
        }

        public double Lat
        {
            get;
            set;
        }


        public double Lng
        {
            get;
            set;
        }


        public string Address
        {
            get;
            set;
        }

        public int Id
        {
            get;
            set;
        }

        
        public int Free
        {
            get;
            set;
        }

        
        public int Total { get; set; }

        public int Ticket { get; set; }
    }
}
