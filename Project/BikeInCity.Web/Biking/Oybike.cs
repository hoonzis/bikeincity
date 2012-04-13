using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BikeInCity.Web.Biking;
using BikeInCity.Model;
using System.Net;
using System.Text.RegularExpressions;

namespace BikeInCity.Web.Biking
{
    public class Oybike : IBikeCity
    {
        public List<City> ProcessCity()
        {
            String url = "http://www.oybike.com/oybike/stands.nsf/getSite?openagent&site=cardiff&format=json&cache=no&extended=yes&key=DFD184FF49E2714410D46E0201E1D17B";

            String getKeyURL = "http://www.oybike.com/oybike.com/oybike/cms.nsf/customFrmMap";

            WebClient client = new WebClient();
            var stationsPage = client.DownloadString(getKeyURL);

            string regexString = @"var mapdbkey = '(?<lat>.*)';"; //var point = new google.maps.LatLng\((?<lat>.*), (?<lng>.*)\);"
            
             
            Regex pointRegex = new Regex(regexString);
            MatchCollection pointMatches = pointRegex.Matches(regexString);

            if (pointMatches.Count == 1)
            {
                var dbkey = pointMatches[0].Groups["key"].Value;
                var json = client.DownloadString(url);

                Console.WriteLine(json);


            }
            return null;
        }
    }
}