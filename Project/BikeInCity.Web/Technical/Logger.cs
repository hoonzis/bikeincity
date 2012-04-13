using System;
using System.IO;
using System.Web;
public static class Logger
{
  public static void WriteMessage(string message)
  {
      
      //String path = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
      
      //String filePath = HttpContext.Current.Server.MapPath("~/logs/log.txt");
      //String filePath = System.Web.Hosting.HostingEnvironment.MapPath("~/logs/log.txt");
      String basePath = AppDomain.CurrentDomain.BaseDirectory;
      String filePath = basePath + "logs\\log.txt";
      try
      {
          StreamWriter sw = new StreamWriter(filePath, true);
          sw.WriteLine(DateTime.Now.ToString("dd/MM/yyyy - HH:mm:ss ") + message);
          sw.Close();
      }
      catch (Exception ex)
      {
          Console.Write(ex.ToString());
      }
  }

  public static void WriteMessage(string filePath, string message)
  {
    

  }
}