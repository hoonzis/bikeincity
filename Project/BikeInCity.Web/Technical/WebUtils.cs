using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Xml.Linq;
using BikeInCity.Model;

namespace BikeInCity.Web.Technical
{
    public static class WebUtils
    {
        public static double ToDoubleTolerate(String input)
        {
            IFormatProvider provider = CultureInfo.InvariantCulture.NumberFormat;
            double output = -1;
            Double.TryParse(input, NumberStyles.Float, provider, out output);
            return output;
        }

        public static List<Station> ProcessManualCity(String path)
        {
            XDocument xDoc = XDocument.Load(path);

            var stations = (from station in xDoc.Descendants("station")
                            select new Station
                            {
                                Address = station.Attribute("name").Value,
                                Lat = WebUtils.ToDoubleTolerate(station.Attribute("lat").Value),
                                Lng = WebUtils.ToDoubleTolerate(station.Attribute("lng").Value),
                                Free = -1
                            }).ToList();
            return stations;

        }

        public static String GetAppDataPath(String fileName)
        {
            String basePath = AppDomain.CurrentDomain.BaseDirectory;
            return basePath + "App_Data\\" + fileName;
        }
    }
}