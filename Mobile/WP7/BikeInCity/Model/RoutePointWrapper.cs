using BikeInCity.ViewModels;
using System;
using BikeInCity.Utils;
using System.Net;
using System.Net.Browser;
using System.Xml.Linq;
using System.IO;
using System.Linq;
using System.Globalization;
using Microsoft.Phone.Controls.Maps.Platform;
using BikeRouter.Utils;

namespace BikeInCity.Model
{
  public class RoutePointWrapper : BaseViewModel
  {
      HttpWebRequest _altitudeRequest;

    private double _altitude;
    public double Altitude
    {
      get { return _altitude; }
      set { 
        _altitude = value;
        OnPropertyChanged(()=>Altitude);
      }
    }

    private Location _pointLocation;
    public Location PointLocation
    {
      get { return _pointLocation; }
      set
      {
        if (_pointLocation != value)
        {
          _pointLocation = value;
        }
      }
    }

    private double _distance;

    public double Distance
    {
      get { return _distance; }
      set {
        if (_distance != value)
        {
          _distance = value;
        }
      }
    }

    public RoutePointWrapper(Location loc)
    {
      _pointLocation = loc;
    }

    public void GetAltitude()
    {
        String latString = Convert.ToString(PointLocation.Latitude, BikeConsts.NFormat);
        String lngString = Convert.ToString(PointLocation.Longitude, BikeConsts.NFormat);

        string webServiceUrl = String.Format("http://api.geonames.org/srtm3XML?lat={0}&lng={1}&username=demo",
          latString, lngString, BikeConsts.NFormat);


        Uri wsUri = new Uri(webServiceUrl, UriKind.Absolute);

        _altitudeRequest = (HttpWebRequest)WebRequestCreator.ClientHttp.Create(wsUri);

        _altitudeRequest.Method = "GET";

        _altitudeRequest.BeginGetResponse(new AsyncCallback(EndGetAltitudeRequest), null);
    }

    private void EndGetAltitudeRequest(IAsyncResult result)
    {

        XDocument doc;
        try
        {
            using (WebResponse response = _altitudeRequest.EndGetResponse(result))
            {

                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        //the service contains the Silverligh privacy policy file so it can be called directly from Silverlight
                        // parse data
                        doc = XDocument.Parse(reader.ReadToEnd());
                        reader.Close();
                    }
                    responseStream.Close();
                }
                response.Close();
            }
            var elevation = from value in doc.Descendants("srtm3") select value;
            //in the case that there will be no elements in the xml just give the error
            if (elevation.Count() > 0)
            {
                double altitude = Convert.ToDouble(elevation.First().Value, CultureInfo.InvariantCulture);
                Altitude = altitude;
            }
            else
            {
                //TODO: Error while getting altitude??
                //IsAltitudeError = true;
            }
        }
        catch (Exception ex)
        {
            //TODO: Catching the strange exception
        }
    }
  }
}
