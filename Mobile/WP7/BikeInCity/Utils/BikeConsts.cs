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
using System.Globalization;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using BikeInCity.ViewModels;
using BikeInCity.Utils;
using BikeInCity.StringResources;
using Microsoft.Phone.Controls.Maps.Platform;

namespace BikeInCity.Utils
{
    public static class BikeConsts
    {

        public const int GPS_SIMULATOR_INTERVAL = 200;
        public const double GPS_SIMULATOR_STEP = 0.015;

        public const double DefaultZoomLevel = 18.0;
        public const double MaxZoomLevel = 21.0;
        public const double MinZoomLevel = 1.0;

        public static NumberFormatInfo NFormat = new NumberFormatInfo() { NumberDecimalSeparator = "."};
        public const int DRIVE_TO_BIKE = 2;
    
        public const string MAPS_KEY = "AlvQY3wmI3KvHvHhqzCw-wuEogZ8SzQs6eqvdzrBPTDLUZiIE9NcoEG_SrHE_xWG";

        public const int STATIONS_COUNT = 5;

        public const int ROUTES_COUNT = 3;

        public const int ZOOM_DETAIL = 16;

        //public const int STATION_DISTANCE = 500;

        public const int TWOPOINTS_LATITUDE_DISTANCE = 180;

        public const String STATIONS_URL_PATTERN = "{0}/Services/BikeWS.svc/json/city?id={1}";
        public const String CITIES_URL_PATTERN = "{0}/Services/BikeWS.svc/json/cities";


        public static String baseUri = "http://www.bikeincity.com";

        public static ObservableCollection<BikeStationViewModel> GetNearStations(Location coordinate, IEnumerable<BikeStationViewModel> allStations, bool from)
        {

            //return collection
            ObservableCollection<BikeStationViewModel> collection = new ObservableCollection<BikeStationViewModel>();

            if (allStations != null)
            {
                List<BikeStationViewModel> stations = null;
                foreach (BikeStationViewModel station in allStations)
                {
                    if (from)
                    {
                        station.WalkDistanceTo = GeoMath.ComputeDistance(station.Location.Latitude, station.Location.Longitude, coordinate.Latitude, coordinate.Longitude);
                    }
                    else
                    {
                        station.WalkDistanceFrom = GeoMath.ComputeDistance(station.Location.Latitude, station.Location.Longitude,coordinate.Latitude, coordinate.Longitude);
                    }
                }

                if (from)
                {
                    stations = allStations.OrderBy(x => x.WalkDistanceTo).Take(5).ToList();
                }
                else
                {
                    stations = allStations.OrderBy(x => x.WalkDistanceFrom).Take(5).ToList();
                }

                foreach (BikeStationViewModel station in stations)
                {
                    collection.Add(station);
                }
            }
            else
            {
                throw new Exception(AppResources.Message_StationsNotLoaded);
            }
            return collection;
        }
    }
}
