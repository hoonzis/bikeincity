<%@ Page Language="C#" MasterPageFile="~/BikeMaster.Master" AutoEventWireup="True"
    Culture="auto" UICulture="auto" CodeBehind="Default.aspx.cs" Inherits="BikeInCity.Web.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="server">
    <link rel="Stylesheet" href="css/default.css" type="text/css" />
    <!-- JS import start -->
    <script type="text/javascript" src="http://maps.googleapis.com/maps/api/js?key=AIzaSyDfMlZQ_3qNhg2Sr8eWgRhtuK7nRKKnGzY&sensor=true"></script>
    <script src="Scripts/jquery-1.7.1.min.js" type="text/javascript"></script>
    <script src="Scripts/jquery-ui-1.8.17.min.js" type="text/javascript"></script>
    <script src="Scripts/modernizr.custom.js" type="text/javascript"></script>
    <script src="Scripts/biking.js" type="text/javascript"></script>
    <script src="Scripts/infobox.js" type="text/javascript"></script>
    <script type="text/javascript">
    var directionsService = new google.maps.DirectionsService();
    var directionsDisplay = new google.maps.DirectionsRenderer();
    var nearStations = [];
    var cities;
    var map;
    var currentCity;
    var stationsArray = [];
    var markersArray = [];
    var routes = [];
    var positionFound = false;
    var currentPosition;
    var latestCenter;
    var cityService = 'Services/Bike.svc/json/cities';
    var stationsService = 'Services/Bike.svc/json/city/';
    var countryService = 'Services/Bike.svc/json/countries';

    //set when clicked on the marker
    var currentStation;
    //departure and arrival position of the route
    var departure;
    var arrival;

    //will hold the current state - looking for stations/or directions or choose city
    var state = 'stations'; //directions/stations/city

    //indicates whether the departure was set
    var departureSet = false;

    //for storing lines drawen on the map
    var lines = [];

    //checks whether the geolocation is possible and asks for current location
    function getCurrentLocation() {
        if (Modernizr.geolocation) {
            navigator.geolocation.getCurrentPosition(posObtained);
        }
    }

    function addStationsToList(elName, st) {
        var stationsBox = document.getElementById(elName);
        stationsBox.innerHTML = "";

        for (var i = 0; i < st.length; i++) {
            var stDist = st[i];
            var freeplaces = 0;
            if (stDist.station.Total != -1) {
                freeplaces = stDist.station.Total - stDist.station.Free;
            }
            stationsBox.innerHTML += "<li id='station" + i + "' onclick='showStation(this)'><b>" + stDist.dist.toFixed() + "</b> m<b><br/>" 
            + stDist.station.Free + "</b> bikes / <b>" + freeplaces + "</b> places" +" </li>";
        }
    }

    function showStation(stationElement) {
        var index = stationElement.id.substring(7, 8);
        var stationMarker = stationsArray[index];

        if (currentStation != null) {
            currentStation.info.close();
        }

        currentStation = nearStations[index].station;
        currentStation.info.open(map, currentStation.marker);
    }

    function computeRoutes() {
        if (currentCity == null || currentCity.Stations == null) {
            return;
        }
        clearArr(stationsArray);
        clearArr(markersArray);

        //erase any routes that were in the list before
        routes = [];
        var nearestDeparture = nearestStations(departure, 2, function (x) { return x.Free > 0; }, currentCity.Stations);
        var nearestArrival = nearestStations(arrival, 2, function (x) { return (x.Total == -1) || (x.Total - x.Free) > 0; }, currentCity.Stations);

        //erase the routes infobox
        var routesInfoBox = document.getElementById("routes");
        routesInfoBox.innerHTML = "";

        //for each station in departure stations/arrival stations get the route
        for (var i = 0; i < nearestDeparture.length; i++) {
            for (var j = 0; j < nearestArrival.length; j++) {
                var dep = nearestDeparture[i];
                var arr = nearestArrival[j];

                var from = new google.maps.LatLng(dep.station.Lat, dep.station.Lng);
                var to = new google.maps.LatLng(arr.station.Lat, arr.station.Lng);

                createStationMarker(dep.station);
                createStationMarker(arr.station);
                calcRoute(from, to);
            }
        }
    }

    function calcRoute(start, end) {
        var request ={ 
            origin: start,
            destination: end,
            travelMode: google.maps.TravelMode.WALKING
        };

        directionsService.route(request, function (result, status) {
            if (status == google.maps.DirectionsStatus.OK) {
                var routeIndex = routes.length;
                routes.push(result);

                //Show the information about the route
                var route = result.routes[0];
                var routesInfoBox = document.getElementById("routes"); //routesInfoBox
                routesInfoBox.innerHTML += "<li id='route" + routeIndex + "' onclick='showRoute(this)'>" + route.legs[0].distance.text + " :: " + route.legs[0].duration.text + "</li>";
            }
        });
    }

    function showRoute(routeDiv) {
        routeIndex = routeDiv.id.substring(5, 6);
        var result = routes[routeIndex];

        //clear existing direction
        directionsDisplay.setMap(null);
        directionsDisplay.setMap(map);
        directionsDisplay.setDirections(result);
    }

    //callback, when current position is obtained

    function posObtained(position) {
        var latitude = position.coords.latitude;
        var longitude = position.coords.longitude;
        var latlng = new google.maps.LatLng(latitude, longitude);
        map.setCenter(latlng);
        if (currentCity == null) {
            //just wait to be sure that we have all the cities
            while (cities == null) { }

            if(cities.length>0){
                currentCity = nearestCity(latitude, longitude, cities);
                //ok found the city - now get stations
                getStations(currentCity.Id);
            }else{
                //TODO: Show error  - no cities loaded
            }
        } else {
            findAndShowNearest(currentPosition);
        }

        //we have position - we can get nearest stations later when they are loaded.
        positionFound = true;
        currentPosition = latlng;
    }

    function setMapToCurrentCity() {
        var latlng = new google.maps.LatLng(currentCity.Lat, currentCity.Lng);
        map.setCenter(latlng);
    }

    function getStations(id) {
        $.getJSON(stationsService + id + "/stations" + "?callback=?", sObtained);
    }

    function sObtained(data) {
        currentCity.Stations = data;
        addNearestToCenterToMap();
    }


    function findAndShowNearest(pos) {
        if (currentCity == null || currentCity.Stations == null) {
            return;
        }

        if (currentStation != null) {
            currentStation.info.close();
        }

        clearArr(lines);
        clearArr(stationsArray);
        
        nearStations = nearestStations(pos, 5, function (x) { return true; }, currentCity.Stations);
        addStationsToList('nearStations', nearStations);
        addStationsToMap(nearStations);
        //$("#nearStations").toggle(true);
    }



    function addStationsToMap(stList) {
        for (var i = 0; i < stList.length; i++) {
            var st = stList[i];
            if (st.dist != null) {
                createStationMarker(st.station);
            } else {
                createStationMarker(st);
            }
        }
    }

    function createStationMarker(stationData) {
        var latlng = new google.maps.LatLng(stationData.Lat, stationData.Lng);
        var image;
        
        if (stationData.Free > 2) {
            image = 'Img/station_green.png';
        }
        else if (stationData.Free <= 0) {
            image = 'Img/station_red.png';
        }
        else {
            image = 'Img/station_orange.png';
        }

        var marker = new google.maps.Marker({
            position: latlng,
            map: map,
            icon: image
        });

        var freeplaces;
        if (stationData.Total != -1) {
            freeplaces = stationData.Total - stationData.Free;
        }else{
            freeplaces = 'not determined'
        }

        
        var boxText = "<div><div style='margin:3px'>" + stationData.Address + '<br/>bikes:' + stationData.Free + '<br/>places:' + freeplaces + "</div></div>";

        var myOptions = {
            content: boxText,
            disableAutoPan: false,
            pixelOffset: new google.maps.Size(10, -45),
            zIndex: null,
            boxClass: "station_infobox",
            closeBoxURL: "",
            infoBoxClearance: new google.maps.Size(1, 1),
            isHidden: false,
            pane: "floatPane",
            enableEventPropagation: false
        };

        var infowindow = new InfoBox(myOptions)
        stationData.info = infowindow;
        stationData.marker = marker;
        google.maps.event.addListener(marker, "click", function () {

            if (currentStation != null) {
                currentStation.info.close();
            }
            currentStation = stationData;
            currentStation.marker = marker;
            currentStation.info = infowindow;
            currentStation.info.open(map, currentStation.marker);
        });
        
        
        stationsArray.push(marker);
    }

    function placeMarker(location) {
        var marker = new google.maps.Marker({
            position: location,
            map: map
        });

        markersArray.push(marker);
    }

    function drawDirectLine(start, end) {
        var routePath = directLine(start, end);
        routePath.setMap(map);
        lines.push(routePath);
    }

    function createMap() {
        var latlng = new google.maps.LatLng(0.0, 0.0);
        var myOptions =
        {
            zoom: 14,
            center: latlng,
            panControl: false,
            zoomControl: true,
            zoomControlOptions: {
                style: google.maps.ZoomControlStyle.BIG,
                position: google.maps.ControlPosition.TOP_RIGHT
            },
            scaleControl: false,
            streetViewControl: false,
            overviewMapControl: false,
            mapTypeControl: false,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };
        map = new google.maps.Map(document.getElementById("map_canvas"), myOptions);
    }

    function addListeners() {
        //add the click handler on the map
        google.maps.event.addListener(map, 'click', function (event) {
            if (state == 'directions' && departureSet == false) {
                departure = event.latLng;
                placeMarker(event.latLng);
                departureSet = true;
            } else if (state == 'directions' && departureSet == true) {
                placeMarker(event.latLng);
                arrival = event.latLng;
                computeRoutes();
                departureSet = false;
                clearArr(lines);
            } else if (state == 'stations') {
                clearArr(lines);
                clearArr(markersArray);
                position = event.latLng;
                placeMarker(event.latLng);
                findAndShowNearest(position);
            }
        });

        //add the mouseover handle over the map
        google.maps.event.addListener(map, 'mousemove', function (event) {
            if (state == 'directions' && departureSet == true) {
                clearArr(lines);
                drawDirectLine(departure, event.latLng);
            }
        });

        google.maps.event.addListener(map, 'center_changed', function () {
            
            var center = map.getCenter();
            if(latestCenter == null){ 
                latestCenter = center;
            }else if(delta(center, latestCenter, 0.004)){
                latestCenter = center;
                addNearestToCenterToMap();
            }
        });
    }

    function addNearestToCenterToMap() {
        if (state == "directions") { return; }
        
        if (currentStation != null) {
            currentStation.info.close();
        }

        if (currentCity != null) {
            clearArr(stationsArray);
            nearStations = nearestStations(map.getCenter(), 50, function (x) { return true; }, currentCity.Stations);
            addStationsToMap(nearStations);
        }
    }
    
    function setClickCallbacks() {
        //control over states
        $("#a_countryList").click(function () {
            state = 'city';
            map.setOptions({ draggableCursor: 'pointer' });
        });
        $("#a_nearStations").click(function () {
            if (currentPosition != null) {
                findAndShowNearest(currentPosition);
            }
            state = 'stations';
            map.setOptions({ draggableCursor: 'crosshair' });
        });
        $("#a_findRoute").click(function () {
            state = 'directions';
            map.setOptions({ draggableCursor: 'crosshair' });
        });

        //Switch the "Open" and "Close" state per click then slide up/down (depending on open/close state)
        $("a.trigger").click(function () {
            $(this).next("ul").toggle("fast");
            $(this).next("ul").siblings("ul").toggle(false);
        });

        $("a.trigger").next("ul").toggle(false);
    }

    function callbackCities(data) {
        cities = data;
        $.each(data, function () {
            $("." + "country" + this.CountryId).append($("<li id='" + this.Id + " ' onclick='setCity(this)' style='cursor:pointer' />").text(this.Name));
        });

        $("ul.ul_city").hide();

        //Show submenu on mouseenter
        $("li.li_country").mouseenter(function () {
            $(this).find("ul").slideToggle("fast");
        });

        //Hide submenu on mouseleave
        $("li.li_country").mouseleave(function () {
            $(this).find("ul").hide();
        });

        getCurrentLocation();
    }

    function setCity(e) {
        getStations(e.id);
        currentCity = findCity(e.id, cities);
        setMapToCurrentCity();
        clearArr(stationsArray);
        clearArr(markersArray);
        //$("a.trigger").next("ul").siblings("ul").toggle(false);
    }

    function callbackCountry(data) {
        var countries = $("#countryList");
        $.each(data, function () {
            var citiesList = "<ul class='ul_city country" + this.Id + "'/>";
            var countryLI = "<li class='li_country'>" + this.Name + "</li>";
            var countryElement = $(countryLI);
            countryElement.append($(citiesList));
            countries.append(countryElement);
        });
    }

    $(document).ready(function () {
        createMap();
        addListeners();
        setClickCallbacks();
        $.getJSON(countryService + "?callback=?", callbackCountry);
        $.getJSON(cityService + "?callback=?", callbackCities);

    });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="map_canvas">
    </div>
    <nav id="menu">
        <a href="#" class="trigger" id="a_countryList">choose city</a><ul id="countryList"
            style="background-color: #808080">
        </ul>
        <a href="#" class="trigger" id="a_nearStations">near stations</a><ul id="nearStations"
            style="background-color: #808080">
        </ul>
        <a href="#" class="trigger" id="a_findRoute">find route</a><ul id="routes" style="background-color: #808080">
            <li>use your mouse to set the arrival and departure</li></ul>
    </nav>
</asp:Content>
