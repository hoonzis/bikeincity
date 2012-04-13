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
using System.Device.Location;
using System.Threading;
using Microsoft.Phone.Controls.Maps;
using BikeRouter.Utils;

namespace BikeInCity.Utils
{
  public class GeoCoordinateSimulator : IGeoPositionWatcher<GeoCoordinate>
  {
    
    //represents left down corner of city bordering rectangle
    private GeoCoordinate _leftCorner;

    //represents right up corner of city bordering rectangle
    private GeoCoordinate _rightCorner;

    //direction in which the current position will change
    private double _dLat;
    private double _dLong;

    //Time interval between 2 changes of position
    private int _interval;

    //height
    private double _latRange;
    //length
    private double _longRange;

    private GeoPosition<GeoCoordinate> _position;
    //timer to fire position changes
    private Timer _timer;
    public Object _timerState;

    /// <summary>
    /// Constructs a simulator which changes current position. Simulates user moving in one directions
    /// until he hits the borders specified by the parameters.
    /// </summary>
    /// <param name="left">Lower left corner of the city</param>
    /// <param name="right">Upper right corner of the city</param>
    /// <param name="interval"></param>
    public GeoCoordinateSimulator(GeoCoordinate center, double radius, int interval)
    {
        _leftCorner = new GeoCoordinate(center.Latitude - radius, center.Longitude - radius);
        _rightCorner = new GeoCoordinate(center.Latitude + radius, center.Longitude - radius);

      //the values should be in between -180 and 180
      if (_leftCorner.Longitude > 180)
      {
        _leftCorner.Longitude -= 360;
      }

      if (_rightCorner.Longitude > 180)
      {
        _leftCorner.Longitude -= 360;
      }

      //check if the city border parameters are correct
      if(_leftCorner.Latitude > _rightCorner.Latitude ||
        _leftCorner.Longitude > _rightCorner.Longitude){
          throw new ArgumentException("Left corner values should be smaller than right corner values");
        }

      _latRange = _rightCorner.Latitude - _leftCorner.Latitude;
      _longRange = _rightCorner.Longitude - _leftCorner.Longitude;

      //setting current position to the midle of the city
      _position = new GeoPosition<GeoCoordinate>(DateTime.Now,
        new GeoCoordinate(_leftCorner.Latitude + _latRange / 2, _leftCorner.Longitude + _longRange / 2));

      //set the interval at which the timer should fire
      _interval = interval;

      //simulator can star right when it is created
      Start();
    }

    #region IGeoPositionWatcher<GeoCoordinate> Members

    public GeoPosition<GeoCoordinate> Position
    {
      get { return _position; }
    }

    public event EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>> PositionChanged;

    
    /// <summary>
    /// According to MSDN:
    /// Start acquiring location data, specifying whether or not to suppress prompting for permissions. This method returns synchronously.
    /// So in case of simulator I can just call the parameterless constructor. This method is not used, but has to be here to implement the interface correctlly.
    /// </summary>
    /// <param name="suppressPermissionPrompt"></param>
    public void Start(bool suppressPermissionPrompt)
    {
      Start();
    }

    /// <summary>
    /// Method to start the simulation - start the timer
    /// </summary>
    public void Start()
    {
      _timer = new Timer(new TimerCallback(TimerCallBack), _timerState, 0, _interval);
    }

    
    public GeoPositionStatus Status
    {
      get {
        if (_timer != null)
        {
          return GeoPositionStatus.Ready;
        }
        return GeoPositionStatus.Disabled;
      }
    }

    public event EventHandler<GeoPositionStatusChangedEventArgs> StatusChanged;

    public void Stop()
    {
      _timer.Dispose();
    }

    /// <summary>
    /// This method is not used in the simulation
    /// </summary>
    /// <param name="suppressPermissionPrompt"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public bool TryStart(bool suppressPermissionPrompt, TimeSpan timeout)
    {
      Start();
      return true;
    }

    #endregion

    /// <summary>
    /// Callback of the timer. This method will set the new position. In most cases it will add the _dLat and _dLong values
    /// to the current position - to simulate the move in the same directions. When it will hit the city borders, it will generate 
    /// new values randomly for the direction. Also the case of where the difference in direction would be zero is not allowed.
    /// </summary>
    /// <param name="obj"></param>
    public void TimerCallBack(Object obj)
    {
      Random r = new Random();
      double newLatitude, newLongitude;

      while (!IsInRange(newLatitude = this.Position.Location.Latitude + _dLat,
      newLongitude = this.Position.Location.Longitude + _dLong) || (_dLat==0.0 && _dLong==0.0))
      {
        _dLat = (r.NextDouble() - 0.5) * BikeConsts.GPS_SIMULATOR_STEP;
        _dLong = (r.NextDouble() - 0.5) * BikeConsts.GPS_SIMULATOR_STEP;
      }

      //set new position
      _position = new GeoPosition<GeoCoordinate>(DateTime.Now, new GeoCoordinate(newLatitude,newLongitude));

      //fire the event if there are any subscribers
      if (this.PositionChanged != null)
      {
        PositionChanged(this, new GeoPositionChangedEventArgs<GeoCoordinate>(this.Position));
      }
    }
   
    /// <summary>
    /// Returns true if the specified position is still in the borders of the city
    /// </summary>
    /// <param name="lat"></param>
    /// <param name="lng"></param>
    /// <returns></returns>
    public bool IsInRange(double lat,double lng){
      return (lat > _leftCorner.Latitude && lng > _leftCorner.Longitude
        && lat < _rightCorner.Latitude && lng < _rightCorner.Longitude) ;
    }
  }
}
