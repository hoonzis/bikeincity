using System;
using System.ComponentModel;
using System.Windows.Media;
using System.Device.Location;
using BikeInCity.Utils;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Xml.Serialization;
using Microsoft.Phone.Controls.Maps.Platform;


namespace BikeInCity.Model
{
  public class BikeStation : INotifyPropertyChanged, IComparable<BikeStation>
  {
    #region Private fields

    private int _free;
    private bool _isSelected;
    private int _walkDistance;    
    private int _id;
    private string _address;
    private int _total;
    private Coordinate _location;
    private ObservableCollection<BikeRoute> _routes;

    #endregion

    public ObservableCollection<BikeRoute> Routes
    {
      get
      {
        if (_routes == null)
        {
          _routes = new ObservableCollection<BikeRoute>();
        }
        return _routes;
      }
      set {
        _routes = value;
        OnPropertyChanged("Routes");
      }
    }

    public bool IsSelected
    {
      get { return _isSelected; }
      set {
        if (_isSelected != value)
        {
          _isSelected = value;
          OnPropertyChanged("IsSelected");
        }
      }
    }

    
    public Coordinate Location
    {
      get
      {
        return _location;
      }
      set
      {
        _location = value;
        OnPropertyChanged("Location");
      }
    }


    public String Address
    {
      get { return _address; }

      set {
        if (value != null)
        {
          string toTrim = " -";


          string val = value;
          if (value.Contains(toTrim))
          {
            val = value.TrimEnd(toTrim.ToCharArray());
          }
          val = val.ToLower();

          if (_address != val)
          {
            _address = val;
            OnPropertyChanged("Address");
          }
        }
      }
    }

    public int Id
    {
      get { return _id; }
      set { _id = value; }
    }

    public int Free
    {
      get { return _free; }
      set { _free = value;
        OnPropertyChanged("Free");
        //OnPropertyChanged("PointColor");
        OnPropertyChanged("FreePlaces");
      }
    }

    public int Total
    {
      get { return _total; }
      set { 
        _total = value;
        OnPropertyChanged("FreePlaces");
      }
    }

    public int WalkDistance
    {
      get { return _walkDistance; }
      set {
        _walkDistance = value;
      }
    }

    public int FreePlaces
    {
      get { return this.Total - this.Free; }
    }

    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    public virtual void OnPropertyChanged(string propertyName)
    {
      PropertyChangedEventHandler handler = this.PropertyChanged;
      if (handler != null)
      {
        var e = new PropertyChangedEventArgs(propertyName);
        handler(this, e);
      }
    }

    #endregion

    #region IComparable<BikeStation> Members

    public int CompareTo(BikeStation other)
    {
      if (this.WalkDistance < other.WalkDistance)
      {
        return -1;
      }
      else if (this.WalkDistance > other.WalkDistance)
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
