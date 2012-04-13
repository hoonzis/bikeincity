using System;
using Microsoft.Phone.Controls.Maps.Platform;
using System.Xml.Serialization;



namespace BikeInCity.Model
{
  public class BikeCity
  {
    public String Name { get; set; }

    //ignore the stations on the serialization - takes too long time to serialize
    [XmlIgnore]
    private BikeStation[] _stations;

    /// <summary>
    /// Empty contstructor is needed for the serialization
    /// </summary>
    public BikeCity()
    {

    }

    public BikeCity(String name,double leftLat, double leftLong,double rightLat,double rightLong,String defAdd1, String defAdd2,int id)
    {
      this.Name = name;
      LeftCorner = new Location();
      LeftCorner.Latitude = leftLat;
      LeftCorner.Longitude = leftLong;
      RightCorner = new Location();
      RightCorner.Longitude = rightLong;
      RightCorner.Latitude = rightLat;
      Add1 = defAdd1;
      Add2 = defAdd2;
      Id = id;
    }

    [XmlIgnore]
    public BikeStation[] Stations {
      get{return _stations;}
      set
      {
        _stations = value;
        if (StationsLoaded != null)
        {
          StationsLoaded(this, null);
        }
      }
    }

    public int Id { get; set; }
    public Location LeftCorner { get; set; }
    public Location RightCorner { get; set; }
    public String Add1 { get; set; }
    public String Add2 { get; set; }

    public event EventHandler StationsLoaded;
  }
}
