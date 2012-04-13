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
using System.Collections.ObjectModel;
using BikeInCity.RouteService;
using Microsoft.Phone.Controls.Maps;
using BikeInCity.Model;
using Microsoft.Phone.Controls.Maps.Platform;
using BikeInCity.Utils;
using System.Linq;

namespace BikeInCity.ViewModels
{
    public class BikeRouteViewModel : BaseViewModel
    {
        public bool AltitudeComputed { get; set; }
        private bool _waitingToGetPoints;

        public BikeRouteViewModel()
        {

        }

        private IRouteService _routeServiceClient;
        public IRouteService RouteServiceClient
        {
            get
            {
                if (_routeServiceClient == null)
                {
                    _routeServiceClient = new RouteServiceClient("BasicHttpBinding_IRouteService");
                }
                return _routeServiceClient;
            }
        }

        private BikeStationViewModel _to;
        public BikeStationViewModel To
        {
            get
            {
                return _to;
            }
            set
            {
                _to = value;
                OnPropertyChanged(() => To);
            }
        }

        private BikeStationViewModel _from;
        public BikeStationViewModel From
        {
            get
            {
                return _from;
            }
            set
            {
                _from = value;
                OnPropertyChanged(() => From);
            }
        }

        private LocationCollection _locations;
        public LocationCollection Locations
        {
            get { return _locations; }
            set
            {
                _locations = value;
                OnPropertyChanged(() => Locations);
            }
        }

        private int _time;
        public int Time
        {
            get
            {
                return _time;
            }
            set
            {
                _time = value;
                OnPropertyChanged(() => Time);
                OnPropertyChanged(() => TotalTime);
            }
        }

        /// <summary>
        /// Total time is the time of the traject + the time you walk to the station and the time, that you walk from the station
        /// </summary>
        public int TotalTime
        {
            get
            {
                return this.Time + this.To.WalkTimeFrom + this.From.WalkTimeTo;
            }
        }

        private double _distance;
        public double Distance
        {
            get { return _distance; }
            set
            {
                if (_distance != value)
                {
                    _distance = value;
                    OnPropertyChanged(() => Distance);
                }
            }

        }


        /// <summary>
        /// Calls a request to bing maps api which will calculate the route between several specified bikesttations.
        /// </summary>
        /// <param name="stations"></param>
        public void CalculateRoute()
        {

            //RouteServiceClient routeClient = new RouteServiceClient("BasicHttpBinding_IRouteService");
            //routeClient.CalculateRouteCompleted += new EventHandler<CalculateRouteCompletedEventArgs>(CalculatedRoute_Completed);

            RouteRequest routeRequest = new RouteRequest();

            routeRequest.Options = new RouteOptions();
            routeRequest.Options.Mode = TravelMode.Driving;
            routeRequest.Options.Optimization = RouteOptimization.MinimizeDistance;
            routeRequest.Options.RoutePathType = RoutePathType.Points;
            routeRequest.Credentials = new Credentials();
            routeRequest.Credentials.ApplicationId = BikeConsts.MAPS_KEY;

            routeRequest.Waypoints = new ObservableCollection<Waypoint>();
            routeRequest.Waypoints.Add(CreateWaypoint(this.From.Location));
            routeRequest.Waypoints.Add(CreateWaypoint(this.To.Location));

            RouteServiceClient.BeginCalculateRoute(routeRequest, new AsyncCallback(EndCalculateRoute), null);
        }

        public void EndCalculateRoute(IAsyncResult e)
        {
            RouteResponse response = RouteServiceClient.EndCalculateRoute(e);
            if ((response.ResponseSummary.StatusCode == RouteService.ResponseStatusCode.Success) &
              (response.Result.Legs.Count != 0))
            {

                LocationCollection locations = new LocationCollection();
                foreach (Location p in response.Result.RoutePath.Points)
                {
                    //add the location also to the location collection
                    locations.Add(p);
                }

                this.Locations = locations;
                this.Distance = response.Result.Summary.Distance;
                this.Time = (int)response.Result.Summary.TimeInSeconds / 60 * BikeConsts.DRIVE_TO_BIKE;

            }
        }

        public Waypoint CreateWaypoint(Location location)
        {
            Waypoint w = new Waypoint();
            w.Location = location;
            return w;
        }
    }
}
