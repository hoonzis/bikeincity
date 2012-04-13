using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using BikeInCity.Model;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Net;
using System.Web.Script.Serialization;
using System.Diagnostics;
using System.Globalization;

namespace BikeInCity.Web.Biking
{
    public class Lyon : IBikeCity
    {
        public List<City> ProcessCity()
        {
            String[] cityParts = { "69381", "69382", "69383", "69384", "69385", "69386", "69387", "69388", "69389", "69266", "69034", "69256" };
            
            City city = new City();
            city.Name = "Lyon";
            city.Stations = new List<Station>();
            
            foreach (var cityPart in cityParts)
            {
                String jsonCity = new WebClient().DownloadString("http://www.velov.grandlyon.com/velovmap/zhp/inc/StationsParArrondissement.php?arrondissement="+cityPart);
                JavaScriptSerializer ser = new JavaScriptSerializer();
                var cityObj = ser.DeserializeObject(jsonCity);

                var members = (Dictionary<String, Object>)cityObj;
                var membersCollection = (Object[])members["markers"];
                foreach (Dictionary<String, Object> marker in membersCollection)
                {
                    try
                    {
                        Station station = new Station();
                        station.Id = Convert.ToInt32(marker["numStation"]);

                        //the update part can be commented out
                        var url = "http://www.velov.grandlyon.com/velovmap/zhp/inc/DispoStationsParId.php?id=" + station.Id + "&noCache=true";
                        XDocument stationDoc = XDocument.Load(url);
                        var updateStation = (from c in stationDoc.Descendants("station")
                                             select new Station
                                             {
                                                 Free = (int)c.Element("available"),
                                                 Total = (int)c.Element("total"),
                                                 Ticket = (int)c.Element("ticket")
                                             }).First();

                        station.Lat = Convert.ToDouble(marker["x"], CultureInfo.InvariantCulture.NumberFormat);
                        station.Lng = Convert.ToDouble(marker["y"], CultureInfo.InvariantCulture.NumberFormat);
                        station.Address = (String)marker["nomStation"];

                        station.Free = updateStation.Free;
                        station.Total = updateStation.Total;

                        city.Stations.Add(station);
                    }
                    catch (Exception ex)
                    {
                        Console.Write("Error Lyon");
                    }
                }
            }

            var cities = new List<City>();
            cities.Add(city);
            return cities;
        }
    }
}