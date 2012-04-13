using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BikeRouter.Web
{
  [DataContract]
  public class Station
  {
    private int _id;
    private string _address;
    private double _lat;
    private double _lng;

    private bool _isUpdate;

    //in case of london or Rennes there is already some information
    //about free places and so on...so set it on
    [DataMember]
    public bool IsUpdate
    {
      get { return _isUpdate; }
      set { _isUpdate = value; }
    }
    
    [DataMember]
    public double Lat
    {
      get { return _lat; }
      set { _lat = value; }
    }

    [DataMember]
    public double Lng
    {
      get { return _lng; }
      set { _lng = value; }
    }

    [DataMember]
    public string Address
    {
      get { return _address; }
      set { _address = value; }
    }

    [DataMember]
    public int Id
    {
      get { return _id; }
      set { _id = value; }
    }

    [DataMember]
    public int Free
    {
      get;
      set;
    }

    [DataMember]
    public int Total { get; set; }

    [DataMember]
    public int Ticket { get; set; }
  }
}
