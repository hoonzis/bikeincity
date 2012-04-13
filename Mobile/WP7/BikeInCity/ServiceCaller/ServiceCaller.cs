using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using BikeInCity.GeocodeService;
using BikeInCity.Model;
using BikeInCity.RouteService;
using BikeInCity.StringResources;
using BikeInCity.Utils;
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Controls.Maps.Platform;
using System.Runtime.Serialization.Json;

namespace BikeInCity.ServiceCaller
{
  public class ServiceCaller
  {
   private string _applicationID;

    private IGeocodeService _geocodeServiceClient;
    public IGeocodeService GeocodeServiceClient
    {
        get
        {
          if (_geocodeServiceClient == null)
            {
              _geocodeServiceClient = new GeocodeServiceClient("BasicHttpBinding_IGeocodeService");
            }
          return _geocodeServiceClient;
        }
    }

    /// <summary>
    /// Constructor - creates an instance of the classes, the application id for bing maps is provided.
    /// </summary>
    /// <param name="appID"></param>
    public ServiceCaller(String appID)
    {
      _applicationID = appID;
    }

   

    

    #region Bing Services Calls

    /// <summary>
    /// Calls bing maps api to geocode address specified in string parametr.
    /// </summary>
    /// <param name="address"></param>
    /// <param name="handler"></param>
    public void GeocodeAddress(string address, EventHandler<GeocodeCompletedEventArgs> handler)
    {
      //if no address was specified, ignore it
      if (address == null || address == String.Empty)
      {
        return;
      }

      GeocodeRequest geocodeRequest = new GeocodeRequest();

      // Set the credentials using a valid Bing Maps key
      geocodeRequest.Credentials = new Credentials();
      geocodeRequest.Credentials.ApplicationId = _applicationID;

      // Set the full address query
      geocodeRequest.Query = address;

      // Set the options to only return high confidence results 
      ConfidenceFilter filter = new ConfidenceFilter();

      filter.MinimumConfidence = GeocodeService.Confidence.High;
      ObservableCollection<FilterBase> filtersCol = new ObservableCollection<FilterBase>();
      filtersCol.Add(filter);


      // Add the filters to the options
      GeocodeOptions geocodeOptions = new GeocodeOptions();
      geocodeOptions.Filters = filtersCol;
      geocodeRequest.Options = geocodeOptions;

      // Make the geocode request
      GeocodeServiceClient geocodeService = new GeocodeServiceClient("BasicHttpBinding_IGeocodeService");

      geocodeService.GeocodeCompleted += handler;
      geocodeService.GeocodeAsync(geocodeRequest);
    }
    
    #endregion

  }
}
