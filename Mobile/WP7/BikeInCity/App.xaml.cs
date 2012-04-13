using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using BikeInCity.Model;
using System.Xml.Serialization;
using System.Globalization;
using System.Threading;
using BikeInCity.Utils;
using BikeInCity.StringResources;
using BikeInCity.ViewModels;

namespace BikeInCity
{
  public partial class App : Application
  {
    /// <summary>
    /// Provides easy access to the root frame of the Phone Application.
    /// </summary>
    /// <returns>The root frame of the Phone Application.</returns>
    public PhoneApplicationFrame RootFrame { get; private set; }

    /// <summary>
    /// Constructor for the Application object.
    /// </summary>
    public App()
    {
      
      //Global handler for uncaught exceptions. 
      UnhandledException += Application_UnhandledException;

      // Show graphics profiling information while debugging.
      if (System.Diagnostics.Debugger.IsAttached)
      {
        // Display the current frame rate counters.
        //Application.Current.Host.Settings.EnableFrameRateCounter = true;

        // Show the areas of the app that are being redrawn in each frame.
        //Application.Current.Host.Settings.EnableRedrawRegions = true;

        // Enable non-production analysis visualization mode, 
        // which shows areas of a page that are being GPU accelerated with a colored overlay.
        //Application.Current.Host.Settings.EnableCacheVisualization = true;
      }

      // Standard Silverlight initialization
      InitializeComponent();

      // Phone-specific initialization
      InitializePhoneApplication();
    }

    // Code to execute when the application is launching (eg, from Start)
    // This code will not execute when the application is reactivated
    private void Application_Launching(object sender, LaunchingEventArgs e)
    {
      //WP7Timer.Start();
      //WP7Timer.Time("Start");

      //provide invariant culture fo deserialization
      Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

      //try to get the situation which was previously saved
      City city = new City();
      try
      {
        using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
        {
          if (isf.FileExists("City.dat"))
          {
            XmlSerializer ser = new XmlSerializer(typeof(City));
            object obj = ser.Deserialize(isf.OpenFile("City.dat", System.IO.FileMode.Open)) as City;
            if (obj != null && obj is City)
            {
                city = obj as City;
                PhoneApplicationService.Current.State.Add("City", city);
            }
          }
        }
      }
      catch (Exception)
      {
        //TODO - what to do with exception in the Launching event??
      }
    }

    // Code to execute when the application is activated (brought to foreground)
    // This code will not execute when the application is first launched
    private void Application_Activated(object sender, ActivatedEventArgs e)
    {
      //I do not take action
      //if there is a situation object it will be loaded in the constructor
    }

    // Code to execute when the application is deactivated (sent to background)
    // This code will not execute when the application is closing
    private void Application_Deactivated(object sender, DeactivatedEventArgs e)
    {
      //during the deactivation save the current situation to the current state object
      MainPage page = RootFrame.Content as MainPage;

      

      //save the last opened city
      if (page.CurrentCity != null && page.CurrentCity.City != null)
      {
          SaveLocalCopy(page.CurrentCity.City);
          var city = page.CurrentCity.City;
          city.FromStations = page.CurrentCity.FromNearStations.Select(x => x.Station).ToList();
          city.CurrentStation = page.CurrentCity.CurrentStation.Station;
          
          city.Lat = page.map.Center.Latitude;
          city.Lng = page.map.Center.Longitude;
          city.Zoom = page.map.ZoomLevel;

          if (PhoneApplicationService.Current.State.ContainsKey("City"))
          {
                PhoneApplicationService.Current.State.Remove("City");
          }
          PhoneApplicationService.Current.State.Add("City", city);
      }
    }

    // Code to execute when the application is closing (eg, user hit Back)
    // This code will not execute when the application is deactivated
    private void Application_Closing(object sender, ClosingEventArgs e)
    {
      //when closing the application save to situation to isolated file storage
      MainPage page = RootFrame.Content as MainPage;      
      SaveLocalCopy(page.CurrentCity.City);
    }

    private void SaveLocalCopy(City city)
    {
      //provde invariant culture for serialization
      Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

      using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
      {
        using (IsolatedStorageFileStream fs = isf.CreateFile("City.dat"))
        {
            XmlSerializer ser = new XmlSerializer(typeof(City));
          ser.Serialize(fs, city);
        }
      }
    }

    // Code to execute if a navigation fails
    private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
    {
      if (System.Diagnostics.Debugger.IsAttached)
      {
        // A navigation has failed; break into the debugger
        System.Diagnostics.Debugger.Break();
      }
    }

    // Code to execute on Unhandled Exceptions
    private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
    {
     
      if (System.Diagnostics.Debugger.IsAttached)
      {
        // An unhandled exception has occurred; break into the debugger
        System.Diagnostics.Debugger.Break();
      }

      e.Handled = true;
      MainPage page = RootFrame.Content as MainPage;
      page.ShowError(AppResources.Message_UnspecifiedError);


    }

    #region Phone application initialization

    // Avoid double-initialization
    private bool phoneApplicationInitialized = false;

    // Do not add any additional code to this method
    private void InitializePhoneApplication()
    {
      if (phoneApplicationInitialized)
        return;

      // Create the frame but don't set it as RootVisual yet; this allows the splash
      // screen to remain active until the application is ready to render.
      RootFrame = new PhoneApplicationFrame();
      RootFrame.Navigated += CompleteInitializePhoneApplication;

      // Handle navigation failures
      RootFrame.NavigationFailed += RootFrame_NavigationFailed;

      // Ensure we don't initialize again
      phoneApplicationInitialized = true;
    }

    // Do not add any additional code to this method
    private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
    {
      // Set the root visual to allow the application to render
      if (RootVisual != RootFrame)
        RootVisual = RootFrame;

      // Remove this handler since it is no longer needed
      RootFrame.Navigated -= CompleteInitializePhoneApplication;
    }

    #endregion
  }
}