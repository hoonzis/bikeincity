using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Microsoft.Phone.Controls.Maps.Platform;
using Microsoft.Phone.Controls.Maps;
using BikeInCity.Model;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization.Json;
using BikeInCity.Utils;
using System.Device.Location;
using BikeInCity.StringResources;

namespace BikeInCity.ViewModels
{    
    public class BikeCityViewModel : BaseViewModel
    {
        HttpWebRequest _stationsRequest;

        public BikeCityViewModel(City city)
        {
            _bikeCity = city;
        }

        private GeoCoordinate _cityCenter;
        public GeoCoordinate CityCenter
        {
            get
            {
                if (_cityCenter == null)
                {
                    _cityCenter = new Location();
                    _cityCenter.Latitude = City.Lat;
                    _cityCenter.Longitude = City.Lng;
                }
                return _cityCenter;
            }
            set { _cityCenter = value; }
        }

        
        public String Name
        {
            get { return _bikeCity.Name; }
            set {
                _bikeCity.Name = value;
                OnPropertyChanged(() => Name);
            }
        }


        private City _bikeCity;
        public City City
        {
            get { return _bikeCity; }
            set
            {
                _bikeCity = value;
                OnPropertyChanged(() => City);
            }
        }

        public bool IsCurrentStation
        {
            get
            {
                return _currentStation != null;
            }
        }

        private ObservableCollection<BikeStationViewModel> _stations;
        public ObservableCollection<BikeStationViewModel> Stations
        {
            get
            {
                return _stations;
            }
            set
            {
                _stations = value;
                OnPropertyChanged(() => Stations);
            }
        }
        private BikeStationViewModel _currentStation;
        public BikeStationViewModel CurrentStation
        {
            get { return _currentStation; }
            set
            {
                if (_currentStation != value)
                {
                    if (_currentStation != null)
                    {
                        _currentStation.IsSelected = false;
                    }
                    _currentStation = value;
                    if (_currentStation != null)
                    {
                        _currentStation.IsSelected = true;
                    }
                    OnPropertyChanged(() => CurrentStation);
                    OnPropertyChanged(() => IsCurrentStation);
                }
            }
        }

        private BikeRouteViewModel _currentRoute;
        public BikeRouteViewModel CurrentRoute
        {
            get
            {
                return _currentRoute;
            }
            set
            {
                if (_currentRoute != value)
                {
                    _currentRoute = value;
                    OnPropertyChanged(() => CurrentRoute);
                    OnPropertyChanged(() => RouteSelected);
                }
            }
        }

        private ObservableCollection<BikeStationViewModel> _fromNearStations;
        public ObservableCollection<BikeStationViewModel> FromNearStations
        {
            get
            {
                return _fromNearStations;
            }
            set
            {
                _fromNearStations = value;
                OnPropertyChanged(() => FromNearStations);
                //select the first station
                if (_fromNearStations != null && _fromNearStations.Count > 0)
                {
                    this.CurrentStation = _fromNearStations[0];
                }

            }
        }

        private ObservableCollection<BikeStationViewModel> _toNearStations;
        public ObservableCollection<BikeStationViewModel> ToNearStations
        {
            get
            {
                //if (_toNearStations == null)
                //{
                //    _toNearStations = new ObservableCollection<BikeStationViewModel>();
                //}
                return _toNearStations;
            }
            set
            {
                if (_toNearStations != value)
                {
                    _toNearStations = value;
                    OnPropertyChanged(() => ToNearStations);
                }
            }
        }


        private ObservableCollection<BikeRouteViewModel> _routes;
        public ObservableCollection<BikeRouteViewModel> Routes
        {
            get
            {
                return _routes;
            }
            set
            {
                if (_routes != value)
                {
                    _routes = value;
                    OnPropertyChanged(() => Routes);
                    OnPropertyChanged(() => RoutesInCity);
                    if (_routes != null && _routes.Count > 0)
                    {
                        this.CurrentRoute = _routes[0];
                    }
                }
            }
        }

        private GeoCoordinate _from;
        public GeoCoordinate From
        {
            get
            {
                //if (_from == null)
                //{
                //    _from = new Location();
                //}
                return _from;
            }
            set
            {
                if (_from != value)
                {
                    _from = value;
                    OnPropertyChanged(() => From);
                }
            }
        }

        private GeoCoordinate _to;
        public GeoCoordinate To
        {
            get
            {
                //if (_to == null)
                //{
                //    _to = new Location();
                //}
                return _to;
            }
            set
            {
                if (_to != value)
                {
                    _to = value;
                    OnPropertyChanged(() => To);

                }
            }
        }

        private bool _showAllWhenLoaded;

        /// <summary>
        /// Calls the proxy client for a list of all bike stations.
        /// </summary>
        /// <param name="completedHandler"></param>
        public void GetAllStations(bool showWhenLoaded)
        {
            _showAllWhenLoaded = showWhenLoaded;
            string url = String.Format(BikeConsts.STATIONS_URL_PATTERN, BikeConsts.baseUri, City.Id);
            _stationsRequest = (HttpWebRequest)HttpWebRequest.Create(new Uri(url, UriKind.Absolute));
            _stationsRequest.Method = "GET";
            _stationsRequest.BeginGetResponse(new AsyncCallback(EndGetAllStations), null);

        }

        public void EndGetAllStations(IAsyncResult result)
        {
            try
            {
                using (WebResponse response = _stationsRequest.EndGetResponse(result))
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {

                        StreamReader reader = new StreamReader(responseStream);
                        string json = reader.ReadToEnd();
                        MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(json));
                        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(City));

                        City city = (City)serializer.ReadObject(stream);
                        if (city != null)
                        {
                            ObservableCollection<BikeStationViewModel> collection = new ObservableCollection<BikeStationViewModel>();

                            for (int i = 0; i < city.Stations.Count; i++)
                            {
                                //stations[i] = new BikeStation(stationsCollection[i])
                                var bikeStation = new BikeStationViewModel(city.Stations[i]);
                                //give them id - just to easy up manipulation
                                bikeStation.Id = i;
                                collection.Add(bikeStation);
                            }

                            this.Stations = collection;
                            if (_showAllWhenLoaded)
                            {
                                this.FromNearStations = collection;
                            }
                        }
                    }
                }
                IsMessage = false;
            }
            catch (Exception ex)
            {
                IsMessage = true;
                Message = AppResources.Message_UnspecifiedError;
            }
        }

        private bool _isMessage;
        public bool IsMessage
        {
            get { return _isMessage; }
            set
            {
                _isMessage = value;
                OnPropertyChanged(() => IsMessage);
            }
        }

        private String _message;

        public String Message
        {
            get { return _message; }
            set { 
                _message = value;
                OnPropertyChanged(() => Message);
            }
        }


        public bool RoutesInCity
        {
            get
            {
                return Routes != null && Routes.Count > 0;
            }
        }

        public bool RouteSelected
        {
            get
            {
                return CurrentRoute != null;
            }
        }

        public double Zoom
        {
            get { return City.Zoom;}
            set {
                var coercedZoom = Math.Max(BikeConsts.MinZoomLevel, Math.Min(BikeConsts.MaxZoomLevel, value));
                if (City.Zoom != coercedZoom)
                {
                    City.Zoom = coercedZoom;
                    OnPropertyChanged(() => Zoom);
                }
            }
        }
    }
    
}
