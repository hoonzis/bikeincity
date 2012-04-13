using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Device.Location;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using BikeInCity.GeocodeService;
using BikeInCity.Model;
using BikeInCity.ServiceCaller;
using BikeInCity.StringResources;
using BikeInCity.Utils;
using BikeInCity.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Linq.Expressions;
using Microsoft.Phone.Controls.Maps.Platform;

namespace BikeInCity
{
  public enum State { Directions, NearStationsTo }

  public partial class MainPage : PhoneApplicationPage, INotifyPropertyChanged
  {
      private HttpWebRequest _citiesRequest;
      private GeoCoordinateWatcher _watcher;
    
      private ServiceCaller.ServiceCaller _serviceCaller;
    
      private bool _trackPosition;
      private State _currentState;

      private ObservableCollection<BikeCityViewModel> _cities;
      public ObservableCollection<BikeCityViewModel> Cities
      {
        get { return _cities; }
        set
        {
          _cities = value;
          OnPropertyChanged(()=>Cities);
        }
      }

      

      #region Constructor
      /// <summary>
      /// Contstructor of the MainPage
      /// </summary>
      public MainPage()
      {
          //initialize the graphic components
          InitializeComponent();
      

          //add the application bar - it has to be done dynamically in order to enable the app bar
          //to be localized
          BuildApplicationBar();

         //tombstonning
         //if there is some Station situation in the State of the application
         if (PhoneApplicationService.Current.State.ContainsKey("City"))
         {
            //get the situation from the current state dictionary - if the app is been run for second time
            var city = PhoneApplicationService.Current.State["City"] as City;
            PhoneApplicationService.Current.State.Remove("City");
            var bikeCity = new BikeCityViewModel(city);


            bikeCity.CityCenter = new GeoCoordinate(city.Lat, city.Lng);

            bikeCity.FromNearStations = new System.Collections.ObjectModel.ObservableCollection<BikeStationViewModel>();

            if (city.FromStations != null)
            {
                foreach (var station in city.FromStations)
                {
                    bikeCity.FromNearStations.Add(new BikeStationViewModel(station));
                }
            }

            if (city.CurrentStation != null)
            {
                bikeCity.CurrentStation = bikeCity.FromNearStations.FirstOrDefault(x => x.Id == city.CurrentStation.Id);
            }

            CurrentCity = bikeCity;
            //silently update stations
            bikeCity.GetAllStations(false);

        }

      //create the GPS watcher for real device and turn it on
      _watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
      _watcher.MovementThreshold = 200;
      SwitchGPS();


      //create new class to access the web services
      _serviceCaller = new BikeInCity.ServiceCaller.ServiceCaller(BikeConsts.MAPS_KEY);
      
      //always load cities
      LoadCities();

      if (CurrentCity == null)
      {
          IsMessage = true;
          Message = AppResources.Message_LoadingCities;
      }

      this.DataContext = this;
    }

    private void LoadCities()
    {
        Uri url = new Uri(String.Format(BikeConsts.CITIES_URL_PATTERN, BikeConsts.baseUri));
        _citiesRequest = (HttpWebRequest)HttpWebRequest.Create(url);
        _citiesRequest.BeginGetResponse(new AsyncCallback(EndGetCities), null);
    }

    private void EndGetCities(IAsyncResult result)
    {
        if (result.IsCompleted)
        {
            try
            {
                List<City> cityList = new List<City>();
                using (WebResponse response = _citiesRequest.EndGetResponse(result))
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<City>));
                        cityList = ((List<City>)serializer.ReadObject(responseStream)).OrderBy(x => x.Name).ToList();
                        

                        var cities = new ObservableCollection<BikeCityViewModel>();
                        foreach (var city in cityList)
                        {
                            cities.Add(new BikeCityViewModel(city));
                        }
                        Cities = cities;       
                    }
                }
                if (CurrentCity == null)
                {
                    CityListVisible = true;
                }
                IsMessage = false;
            }
            catch (Exception ex)
            {
                IsMessage = true;
                Message = AppResources.Message_UnspecifiedError;
            }
        }
    }


    #endregion

      #region INotifyPropertyChanged Members

      public event PropertyChangedEventHandler PropertyChanged;

      protected void OnPropertyChanged<T>(Expression<Func<T>> property)
      {
          var expression = property.Body as MemberExpression;
          var member = expression.Member;

          if (this.PropertyChanged != null)
          {
              Deployment.Current.Dispatcher.BeginInvoke(() =>
              {
                  if (this.PropertyChanged != null)
                  {
                      this.PropertyChanged(this, new PropertyChangedEventArgs(member.Name));
                  }
              });
          }
      }

      #endregion

      #region Button Handlers

      private void ButtonZoomIn_Click(object sender, RoutedEventArgs e)
      {
        CurrentCity.Zoom += 1;
      }

      private void ButtonZoomOut_Click(object sender, RoutedEventArgs e)
      {
          //map.ZoomLevel -= 1;
          CurrentCity.Zoom -= 1;
      }

    private void ComputeDirections_Click(object sender, RoutedEventArgs e)
    {
      //hide the directions panel
      DirectionsPanel.Visibility = System.Windows.Visibility.Collapsed;

      string address = String.Empty;

      //if the address does not contain the name of the city, we will add it
      if (!this.AddressTextBox.Text.Contains(CurrentCity.Name))
      {
        address = String.Format("{0},{1}", this.AddressTextBox.Text, CurrentCity.Name);
      }
      else
      {
        address = this.AddressTextBox.Text;
      }


      //call the geocode
      if (_currentState == BikeInCity.State.Directions)
      {
          _serviceCaller.GeocodeAddress(address, GeocodeForDirectionsCompleted);
      }
      else
      {
          _serviceCaller.GeocodeAddress(address, GeocodeForStationsCompleted);
      }

    }

    public void GeocodeForDirectionsCompleted(object sender, GeocodeCompletedEventArgs e)
    {

        if (e.Result.ResponseSummary.StatusCode != ResponseStatusCode.Success)
        {
            IsMessage = true;
            Message = AppResources.Message_AddressNotFound;
            return;
        }

        if (e.Result.Results.Count == 0 || e.Result.Results[0].Locations.Count == 0)
        {
            IsMessage = true;
            Message = AppResources.Message_AddressNotFound;
            return;
        }

        //set the destination place
        CurrentCity.To = e.Result.Results[0].Locations[0];
            

        //retrieve the stations arround destination place
        CurrentCity.ToNearStations = BikeConsts.GetNearStations(CurrentCity.To, CurrentCity.Stations, false);

        //if there was any station before selected than i can compute directions
        if (CurrentCity.CurrentStation != null)
        {
            var routes = new ObservableCollection<BikeRouteViewModel>();

            
            foreach (var finish in CurrentCity.ToNearStations)
            {
                BikeRouteViewModel route = new BikeRouteViewModel();
                route.From = CurrentCity.CurrentStation;
                route.To = finish;
                route.CalculateRoute();
                routes.Add(route);
                    
            }

            CurrentCity.Routes = routes;
        }
        
    }

    private void GeocodeForStationsCompleted(object sender, GeocodeCompletedEventArgs e)
    {
        if (e.Result.ResponseSummary.StatusCode != ResponseStatusCode.Success)
        {
            IsMessage = true;
            Message = AppResources.Message_AddressNotFound;
            return;
        }

        if (e.Result.Results.Count == 0 || e.Result.Results[0].Locations.Count == 0)
        {
            IsMessage = true;
            Message = AppResources.Message_AddressNotFound;
            return;
        }

        //change the from location
        CurrentCity.From = e.Result.Results[0].Locations[0];
        //load the near stations
        ShowNearStations();

        //erase the current selected route
        CurrentCity.CurrentRoute = null;
    }

    public void ShowNearStations()
    {
        if (CurrentCity.Stations == null)
        {
            return;
        }

        CurrentCity.FromNearStations = BikeConsts.GetNearStations(CurrentCity.From, CurrentCity.Stations, true);
        //this.map.SetView(CurrentCity.From, BikeConsts.ZOOM_DETAIL);
    }

    private void Continue_Click(object sender, RoutedEventArgs e)
    {
      this.HideInfoBox();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
      this.HideInfoBox();
    }

    #endregion

      #region AppBar Buttons & Menus Handlers
    
    /// <summary>
    /// Shows overlay with the list of all the cities
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Cities_Click(object sender, EventArgs e)
    {
        CityListVisible = true;
    }

    /// <summary>
    /// Handles the click on the Directions icon in application bar
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void GetDirections_Click(object sender, EventArgs e)
    {
      this.DirectionsPanel.Visibility = System.Windows.Visibility.Visible;
      _currentState = BikeInCity.State.Directions;
      this.ToTextBox.Text = AppResources.TextBoxTo;
      
    }

    /// <summary>
    /// Handles the click on the "Address" icon in application bar.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void NearStationsTo_Click(object sender, EventArgs e)
    {
      this.DirectionsPanel.Visibility = System.Windows.Visibility.Visible;
      _currentState = BikeInCity.State.NearStationsTo;
      this.ToTextBox.Text = AppResources.TextBoxNearTo;
    }

    /// <summary>
    /// Handles the click on the "Here" icon in application bar
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ShowNearStations_Click(object sender, EventArgs e)
    {
        if (_watcher == null)
        {
            ShowError(AppResources.Message_GPS_OFF);
        }
      
        switch (_watcher.Status)
        {
          case GeoPositionStatus.Disabled: ShowError(AppResources.Message_GPS_OFF); break;
          case GeoPositionStatus.NoData: ShowError(AppResources.Message_GPS_NoData); break;
          case GeoPositionStatus.Initializing: ShowError(AppResources.Message_GPS_Init); break;
          case GeoPositionStatus.Ready:
          {
              CurrentCity.From = _watcher.Position.Location;
              ShowNearStations();
          }break;
        }
    }

    public void ShowError(String err)
    {
        IsMessage = true;
        Message = err;
    }

    /// <summary>
    /// Turn on / off the position tracking, just subscribes to the PositionChanged event of GeoCoordinateWatcher.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TrackPosition_Click(object sender, EventArgs e)
    {
      SwitchGPS();
      //this won't work, probably a bug so you have to use sender object
      //MenuGPSTracking.Text = _trackPosition ? "GPS Tracking: On" : "GPS Tracking:Off";
      //probably because AppBar is not silverlight
      ((ApplicationBarMenuItem)sender).Text = _trackPosition ? AppResources.AppBar_TurnOffGPS : AppResources.AppBar_TurnOnGPS;
    }

    /// <summary>
    /// Method which builds application bar for this application.
    /// It has to be done dynamically in order to enable localization
    /// </summary>
    private void BuildApplicationBar()
    {
      // Set the page's ApplicationBar to a new instance of ApplicationBar
      ApplicationBar = new ApplicationBar();

      ApplicationBarIconButton appBarDirections = new ApplicationBarIconButton(new Uri("/Icons/ApplicationBar/Directions.png", UriKind.Relative));
      appBarDirections.Text = AppResources.AppBar_Directions;
      appBarDirections.Click += new EventHandler(GetDirections_Click);
      ApplicationBar.Buttons.Add(appBarDirections);

      ApplicationBarIconButton appBarHere = new ApplicationBarIconButton(new Uri("/Icons/ApplicationBar/Location.png", UriKind.Relative));
      appBarHere.Text = AppResources.AppBar_Here;
      appBarHere.Click += new EventHandler(ShowNearStations_Click);
      ApplicationBar.Buttons.Add(appBarHere);

      ApplicationBarIconButton appBarSearch = new ApplicationBarIconButton(new Uri("/Icons/ApplicationBar/Search.png", UriKind.Relative));
      appBarSearch.Text = AppResources.AppBar_Search;
      appBarSearch.Click += new EventHandler(NearStationsTo_Click);
      ApplicationBar.Buttons.Add(appBarSearch);


      ApplicationBarIconButton appBarShowAll = new ApplicationBarIconButton(new Uri("/Icons/ApplicationBar/All.png", UriKind.Relative));
      appBarShowAll.Text = AppResources.AppBar_All;
      appBarShowAll.Click += new EventHandler(ShowAll_Click);
      ApplicationBar.Buttons.Add(appBarShowAll);
      //Creating menu items with localized strings from AppResources
      
      //change city
      ApplicationBarMenuItem appBarMenuCity = new ApplicationBarMenuItem(AppResources.AppBar_ChangeCity);
      appBarMenuCity.Click+= new EventHandler(Cities_Click);
      ApplicationBar.MenuItems.Add(appBarMenuCity);

      //Turn off gps
      ApplicationBarMenuItem appBarMenuGPS = new ApplicationBarMenuItem(AppResources.AppBar_TurnOffGPS);
      appBarMenuGPS.Click += new EventHandler(TrackPosition_Click);
      ApplicationBar.MenuItems.Add(appBarMenuGPS);

      //about
      ApplicationBarMenuItem appBarMenuAbout = new ApplicationBarMenuItem(AppResources.AppBar_About);
      appBarMenuAbout.Click += new EventHandler(About_Click);
      ApplicationBar.MenuItems.Add(appBarMenuAbout);

      //privacy policy
      ApplicationBarMenuItem appBarMenuPrivacyPolicy = new ApplicationBarMenuItem(AppResources.AppBar_PrivacyPolicy);
      appBarMenuPrivacyPolicy.Click += new EventHandler(PrivacyPolicy_Click);
      ApplicationBar.MenuItems.Add(appBarMenuPrivacyPolicy);
    }

    /// <summary>
    /// Show the Privacy Policy regarding the usage of GPS data.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void PrivacyPolicy_Click(object sender, EventArgs e)
    {
      NavigationService.Navigate(new Uri("/Pages/PolicyPage.xaml", UriKind.Relative));
    }

    private void ShowAll_Click(object sender, EventArgs e)
    {
        CurrentCity.FromNearStations = CurrentCity.Stations;
    }

    /// <summary>
    /// Show the information about the application.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void About_Click(object sender, EventArgs e)
    {
      NavigationService.Navigate(new Uri("/Pages/AboutPage.xaml", UriKind.Relative));
    }

    private void SwitchGPS()
    {
        if (_trackPosition)
        {
            _watcher.Stop();
        }
        else
        {
            _watcher.Start();
        }
        _trackPosition = !_trackPosition;
    }

    #endregion

      #region Utils

    public void HideInfoBox()
    {
        CurrentCity.IsMessage = false;
        IsMessage = false;
    }

      #endregion

      #region Page events
    
    /// <summary>
    /// Overrides the general method for pressing the Back button. If the cities or address panel is currently open,
    /// it will be closed instead of leaving the application.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnBackKeyPress(CancelEventArgs e)
    {
        if (IsMessage)
        {
            IsMessage = false;
            e.Cancel = true;
        }

        if (CurrentCity.IsMessage)
        {
            CurrentCity.IsMessage = false;
            e.Cancel = true;
        }

        if (CityListVisible)
        {
            CityListVisible = false;
            e.Cancel = true;
        }
      
        if(this.DirectionsPanel.Visibility == System.Windows.Visibility.Visible){
          this.DirectionsPanel.Visibility = System.Windows.Visibility.Collapsed;
          e.Cancel = true;
        }
    }

    protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
    {
        _watcher.Dispose();
        base.OnNavigatedFrom(e);
    }

    #endregion

      private void Textbox_GotFocus(object sender, RoutedEventArgs e)
      {
        TextBox textbox = sender as TextBox;
        textbox.SelectAll();
      }

      private void Rectangle_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
      {
         e.Handled = true;
         //set the current station as selected
         BikeStationViewModel station = (BikeStationViewModel)((Border)sender).DataContext;
         CurrentCity.CurrentStation = station;
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

      private bool _isMessage;
      public bool IsMessage
      {
        get { return _isMessage; }
        set {
            _isMessage = value;
            OnPropertyChanged(() => IsMessage);
        }
      }

      private bool _cityListVisible;

      public bool CityListVisible
      {
          get { return _cityListVisible; }
          set { 
              _cityListVisible = value;
              OnPropertyChanged(()=>CityListVisible);
          }
      }

      private BikeCityViewModel _currentCity;

      public BikeCityViewModel CurrentCity
      {
          get { return _currentCity; }
          set { 
              _currentCity = value;
              OnPropertyChanged(() => CurrentCity);

              CityListVisible = false;
              map.Center = CurrentCity.CityCenter;
              
              if (CurrentCity.Zoom == 0)
              {
                  CurrentCity.Zoom = BikeConsts.DefaultZoomLevel;
              }

              if (_currentCity != null && _currentCity.Stations == null || _currentCity.Stations.Count < 0)
              {
                  _currentCity.IsMessage = true;
                  _currentCity.Message = AppResources.Message_LoadingStations;
                  _currentCity.GetAllStations(false);
              }
          }
      }
  }
}
