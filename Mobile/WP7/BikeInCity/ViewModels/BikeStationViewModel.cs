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
using Microsoft.Phone.Controls.Maps.Platform;
using System.Device.Location;

namespace BikeInCity.ViewModels
{
    public class BikeStationViewModel : BaseViewModel
    {
        private Station _station;

        public Station Station
        {
            get { return _station; }
            set
            {
                _station = value;
                OnPropertyChanged(() => Station);
            }
        }


        public BikeStationViewModel(Station station)
        {
            Station = station;

        }

        /// <summary>
        /// The time needed to walk the distance to this station.
        /// you do 200m in 2 minutes..reasonable
        /// </summary>
        public int WalkTimeTo
        {
            get { return WalkDistanceTo / 100; }
        }

        /// <summary>
        /// The time needed to walk the distance from the station to desired position
        /// </summary>
        public int WalkTimeFrom
        {
            get { return WalkDistanceFrom / 100; }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged(() => IsSelected);
            }
        }

        private GeoCoordinate _location;

        public GeoCoordinate Location
        {
            get
            {
                if (_location == null && Station != null)
                {
                    _location = new GeoCoordinate(Station.Lat, Station.Lng);
                }
                return _location;
            }
        }

        public String Address
        {
            get { return Station.Address; }

            set
            {
                if (value != null)
                {
                    string toTrim = " -";


                    string val = value;
                    if (value.Contains(toTrim))
                    {
                        val = value.TrimEnd(toTrim.ToCharArray());
                    }
                    val = val.ToLower();

                    if (Station.Address != val)
                    {
                        Station.Address = val;
                        OnPropertyChanged(() => Address);
                    }
                }
            }
        }

        public int Id
        {
            get { return Station.Id; }
            set
            {
                if (Station.Id != value)
                {
                    Station.Id = value;
                    OnPropertyChanged(() => Id);
                }
            }
        }

        public int Free
        {
            get { return Station.Free; }
            set
            {
                Station.Free = value;
                OnPropertyChanged(() => Free);
                OnPropertyChanged(() => FreePlaces);
                OnPropertyChanged(() => IsEmpty);
            }
        }

        public bool IsEmpty
        {
            get
            {
                return Free == 0;
            }
        }

        private int _walkDistanceTo;
        public int WalkDistanceTo
        {
            get { return _walkDistanceTo; }
            set
            {
                if (_walkDistanceTo != value)
                {
                    _walkDistanceTo = value;
                    OnPropertyChanged(() => WalkDistanceTo);
                }
            }
        }

        private int _walkDistanceFrom;
        public int WalkDistanceFrom
        {
            get { return _walkDistanceFrom; }
            set
            {
                if (_walkDistanceFrom != value)
                {
                    _walkDistanceFrom = value;
                    OnPropertyChanged(() => WalkDistanceFrom);
                }
            }
        }

        public String FreePlaces
        {

            get
            {
                if (_station.Total == -1)
                {
                    return "?";
                }
                return Convert.ToString(_station.Total - _station.Free);
            }
        }

        public int Total
        {
            get
            {
                return _station.Total;
            }
            set
            {
                if (_station.Total != value)
                {
                    _station.Total = value;
                    OnPropertyChanged(() => Total);
                    OnPropertyChanged(() => FreePlaces);
                }
            }
        }
    }
}