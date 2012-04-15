using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BikeInCity.Model;
using System.Xml.Linq;
using log4net;

namespace BikeInCity.Web.Biking
{
    public static class Decaux
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Decaux));

        /// <summary>
        /// Gets the data about Decaux working service. Some cities require postfixing the stationdetails url by the name of the city.
        /// .../stationdetails/valence/id
        /// ../stationndetaisl/id
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="stationDetailsPostfix"></param>
        /// <returns></returns>
        public static City GetCity(String baseUrl, String stationDetailsPostfix)
        {
            XDocument xDoc = XDocument.Load(baseUrl + "service/carto");

            var stations = (from c in xDoc.Descendants("carto").Descendants("markers").Descendants("marker")
                            select new Station
                            {
                                Address = ((string)c.Attribute("address")).Replace("-","").ToLower(),
                                Id = (int)c.Attribute("number"),
                                Lat = (double)c.Attribute("lat"),
                                Lng = (double)c.Attribute("lng"),
                                //initialize Free to -1 - if the client obtains -1 it will show it orange - as we do not have the correct information
                                Free = -1,
                                //no info yet
                                IsUpdate = false
                            }).ToList();


            try
            {
                //update values for each station
                foreach (var station in stations)
                {
                    String url = baseUrl + "service/stationdetails/" + stationDetailsPostfix + station.Id;
                    XDocument stationDoc = XDocument.Load(url);
                    var updateStation = (from c in stationDoc.Descendants("station")
                                         select new Station
                                         {
                                             Free = (int)c.Element("available"),
                                             Total = (int)c.Element("total"),
                                             Ticket = (int)c.Element("ticket")
                                         }).First();

                    station.Free = updateStation.Free;
                    station.Total = updateStation.Total;

                }
            }
            catch (Exception ex)
            {
                _log.Info("Decaux: - Error while getting information about station: " + ex);
            }
            
            City city = new City
            {
                Stations = stations,
                TimeStamp = DateTime.Now
            };

            return city;
        }
    }
}