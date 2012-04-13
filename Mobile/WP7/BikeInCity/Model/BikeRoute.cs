using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using BikeInCity.Utils;
using System.Windows.Media;
using Microsoft.Phone.Controls.Maps;
using System.Xml.Serialization;
using System.Device.Location;
using System.Collections.Generic;

namespace BikeInCity.Model
{
  public class BikeRoute : INotifyPropertyChanged, IComparable<BikeRoute>
  {
    #region Private fields
    private Coordinate _fromLocation;
    //private Coordinate _toLocation;
    private int _fromWalkDistance;
    //private int _toWalkDistance;
    private BikeStation _to;
    //private BikeStation _from;
    private double _distance;
    private int _time;
    private List<Coordinate> _locations;
    private bool _isSelected;
    private int _totalTime;
    #endregion


    #region Ctor
    public BikeRoute()
    {
      _locations = new List<Coordinate>();
      _isSelected = false;
    }
    #endregion

    #region Public Properties

    public bool IsSelected
    {
      get { return _isSelected; }
      set { 
        _isSelected = value;
        OnPropertyChanged("Opacity");
      }
    }

    public int TotalTime
    {
      get
      {
        return _totalTime;
      }
      set
      {
        _totalTime = value;
        OnPropertyChanged("TotalTime");
      }
    }

    public double TotalDistance
    {
      get { return this.Distance + this.FromWalkDistance + this.To.WalkDistance; }
    }

    
    public BikeStation To
    {
      get { return _to; }
      set { _to = value; }
    }


    public Coordinate FromLocation
    {
      get { return _fromLocation; }
      set { _fromLocation = value; }
    }

    /*
    public Coordinate ToLocation
    {
      get { return _toLocation; }
      set { _toLocation = value; }
    }*/

    public int FromWalkDistance
    {
      get { return _fromWalkDistance; }
      set { _fromWalkDistance = value; }
    }

    /*
    public int ToWalkDistance
    {
      get { return _toWalkDistance; }
      set { _toWalkDistance = value; }
    }*/

    //Have to ignore this, because it would cause circular reference (Station -> Route -> Station)
    //but as you see it is important for computation of total tiem
      /*
    [XmlIgnore]
    public BikeStation From
    {
      get { 
        if(_from == null){
          _from = new BikeStation();
        }
        return _from; 
      }
      set { _from = value; }
    }*/

    
    public List<Coordinate> Locations
    {
      get
      {
        return _locations;
      }
      set
      {
        _locations = value;
      }
    }

    public int Time
    {
      get { return _time; }
      set
      {
        if (_time != value)
        {
          _time = value;
          UpdateTotalTime();
          OnPropertyChanged("Time");
        }
      }
    }

    public double Distance
    {
      get { return _distance; }
      set
      {
        if (_distance != value)
        {
          _distance = value;
          OnPropertyChanged("Distance");
        }
      }
    }

    #endregion

    public void UpdateTotalTime()
    {
      if(this.To!=null){
        this.TotalTime = this.Time + (int)(this.To.WalkDistance * BikeConst.DIST_TO_TIME) + (int)(this.FromWalkDistance * BikeConst.DIST_TO_TIME);
      }
    }

    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
      PropertyChangedEventHandler handler = this.PropertyChanged;
      if (handler != null)
      {
        var e = new PropertyChangedEventArgs(propertyName);
        handler(this, e);
      }
    }
    #endregion

    #region IComparable<BikeRoute> Members

    public int CompareTo(BikeRoute other)
    {
      if (this.TotalTime < other.TotalTime)
      {
        return -1;
      }
      else if (this.TotalTime > other.TotalTime)
      {
        return 1;
      }
      else
      {
        return 0;
      }
    }

    #endregion
  }
}
