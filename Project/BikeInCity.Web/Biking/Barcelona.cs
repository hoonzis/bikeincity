using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using BikeInCity.Model;

using System.Globalization;

namespace BikeInCity.Web.Biking
{
    public class Barcelona : IBikeCity
    {
        public List<City> ProcessCity()
        {
            //iformatprovide for the convert method
            IFormatProvider provider = CultureInfo.InvariantCulture.NumberFormat;

            WebClient client = new WebClient();
            var xmlString = client.DownloadString("http://www.bicing.cat/localizaciones/localizaciones.php");

            //regex to find the KML data in the HTML
            Regex kmlRegex = new Regex("<Document>(.*)</Document>");

            //finds the DIV tag containing the address
            Regex addressTerm = new Regex("<div style=\"font:bold 11px verdana;color:#cf152c;margin-bottom:10px\">(?<match>[^<]*?)</div>");

            //finds the DIV tag containing to digits with information of free bikes and free places
            Regex freeTerm = new Regex("<div style=\"margin-left:5px;float:left;font:bold 11px verdana;color:green\">(?<free>\\d*)<br />(?<places>\\d*)<br /></div>");

            Match m = kmlRegex.Match(xmlString);

            String kmlData = m.Value;
            XDocument xDoc = XDocument.Parse(kmlData);

            char[] separator = { ',' };

            var test = (from c in xDoc.Descendants("Document").Descendants("Placemark")

                        let description = (string)c.Element("description").Value
                        let addressMatch = addressTerm.Match(description)
                        let address = addressMatch.Groups["match"].Value

                        let freeMatch = freeTerm.Match(description)
                        let free = freeMatch.Groups["free"].Value
                        let places = freeMatch.Groups["places"].Value

                        let coordinates = c.Element("Point").Element("coordinates").Value
                        let coordinatesarray = coordinates.Split(separator)

                        //here I am rather creating anonymous type, because the convert methods might cause exceptions
                        //which I do not know how to catch i LINQ
                        select new
                        {
                            free,
                            places,
                            address,
                            coordinatesarray
                        });

            //have to initialize the list(which is done automatically by LINQ in cases of other cities
            var stations = new List<Station>();

            foreach (var v in test)
            {
                try
                {
                    //prepare the values
                    double lat = Convert.ToDouble(v.coordinatesarray[1], provider);
                    double lng = Convert.ToDouble(v.coordinatesarray[0], provider);

                    int free = Convert.ToInt16(v.free, provider);
                    int total = free + Convert.ToInt16(v.places, provider);

                    //create new station and add to the result lit
                    stations.Add(new Station()
                    {
                        Lat = lat,
                        Lng = lng,
                        Address = v.address,
                        Free = free,
                        Total = total,
                        IsUpdate = true
                    });
                }
                //here catch the possible errors of conversions
                catch (Exception ex)
                {
                    Logger.WriteMessage("Error while converting Barcelona station.\n" + ex.Message);
                }
            }

            String name = "Barcelona";
            City city = new City
            {
                Name = name,
                Stations = stations,
                TimeStamp = DateTime.Now
            };

            var cities = new List<City>();
            cities.Add(city);
            return cities;
        }
    }
}