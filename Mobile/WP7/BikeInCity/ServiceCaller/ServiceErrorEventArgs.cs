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

namespace BikeInCity.ServiceCaller
{
  public class ServiceErrorEventArgs : EventArgs
  {
    public String Details { get; set; }

    public ServiceErrorEventArgs(String details)
    {
      this.Details = details;
    }
  }
}
