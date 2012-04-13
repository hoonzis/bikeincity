using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using BikeInCity.Model;

namespace BikeInCity.Web.Biking
{
    public class Denver : IBikeCity
    {

        public List<City> ProcessCity()
        {
            //iformatprovide for the convert method
            IFormatProvider provider = CultureInfo.InvariantCulture.NumberFormat;

            var stations = new List<Station>();


            WebClient client = new WebClient();
            var xmlString = client.DownloadString("http://denver.bcycle.com/");

            string regexString = @"var point = new google.maps.LatLng\((?<lat>.*), (?<lng>.*)\);"
            + @".*\n.*kioskpoints.push\(point\);"
            + ".*\n.*var marker = new createMarker\\(point, \"<div class='location'><strong>.*</strong><br />(?<adr>.*)<br />.*</div>"
            + "<div class='avail'>Bikes available: <strong>(?<bikes>.*)</strong><br />Docks available: <strong>(?<docks>.*)</strong></div>";
            //finds 
            Regex pointRegex = new Regex(regexString);
            MatchCollection pointMatches = pointRegex.Matches(xmlString);
            foreach (Match match in pointMatches)
            {
                double latitude = Double.Parse(match.Groups["lat"].Value, provider);
                double longitude = Double.Parse(match.Groups["lng"].Value, provider);
                int bikes = Int16.Parse(match.Groups["bikes"].Value, provider);
                int docks = Int16.Parse(match.Groups["docks"].Value, provider);
                String adress = match.Groups["adr"].Value;
                Station station = new Station { Lat = latitude, Lng = longitude, Free = bikes, Address = adress, Total = bikes + docks, IsUpdate = true };
                stations.Add(station);
            }
            String name = "Denver";
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