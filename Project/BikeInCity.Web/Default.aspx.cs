using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Threading;

namespace BikeInCity.Web
{
  public partial class Default : System.Web.UI.Page
  {
      public String culture;
      public String uiculture;
      protected override void OnLoad(EventArgs e)
      {
          base.OnLoad(e);
          CultureInfo cult = Thread.CurrentThread.CurrentCulture;
          CultureInfo uicult = Thread.CurrentThread.CurrentUICulture;

          culture = cult.ToString();
          uiculture = uicult.ToString();

          Console.WriteLine(cult.ToString() + uicult.ToString());
      }
  }
}