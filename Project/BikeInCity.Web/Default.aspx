<%@ Page Language="C#" MasterPageFile="~/BikeMaster.Master" AutoEventWireup="True"
    Culture="auto" UICulture="auto" CodeBehind="Default.aspx.cs" Inherits="BikeInCity.Web.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="server">
    <link rel="Stylesheet" href="css/default.css" type="text/css" />
    <!-- JS import start -->
    <script type="text/javascript" src="http://maps.googleapis.com/maps/api/js?key=AIzaSyDp5dwn-A6d6TvpMoFXe-ea4WYBB5VveHc&sensor=true"></script>
    <script src="Scripts/jquery-1.7.1.min.js" type="text/javascript"></script>
    <script src="Scripts/jquery-ui-1.8.17.min.js" type="text/javascript"></script>
    <script src="Scripts/modernizr.custom.js" type="text/javascript"></script>
    <script src="/Scripts/knockout-2.0.0.debug.js" type="text/javascript"></script>
    <script src="/Scripts/knockout.mapping-latest.debug.js" type="text/javascript"></script>
    <script src="Scripts/biking.js" type="text/javascript"></script>
    <script src="Scripts/infobox.js" type="text/javascript"></script>
    <!--ViewModels-->
    <script src="ViewModels/CityViewModel.js" type="text/javascript"></script>
    <script src="ViewModels/CountryListViewModel.js" type="text/javascript"></script>
    <script src="ViewModels/CountryViewModel.js" type="text/javascript"></script>
    <script src="ViewModels/StationViewModel.js" type="text/javascript"></script>
    <script src="ViewModels/TipViewModel.js" type="text/javascript"></script>
    <script type="text/javascript">

        var _gaq = _gaq || [];
        _gaq.push(['_setAccount', 'UA-31896309-1']);
        _gaq.push(['_trackPageview']);

        (function () {
            var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
            ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
            var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
        })();
    </script>
    <script type="text/javascript">
    var countryList;
    var directionsService = new google.maps.DirectionsService();
    var directionsDisplay = new google.maps.DirectionsRenderer();
    var nearStations = [];
    var map;    
    var stationsArray = [];
    var markersArray = [];
    var routes = [];
    var positionFound = false;
    var currentPosition;
    var latestCenter;

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
            if (stDist.station.total() != -1) {
                freeplaces = stDist.station.total() - stDist.station.free();
            }
            stationsBox.innerHTML += "<li id='station" + i + "' onclick='showStation(this)'><b>" + stDist.dist.toFixed() + "</b> m<b><br/>" 
            + stDist.station.free() + "</b> bikes / <b>" + freeplaces + "</b> places" +" </li>";
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
        clearArr(stationsArray);
        clearArr(markersArray);

        //erase any routes that were in the list before
        routes = [];
        var nearestDeparture = nearestStations(departure, 3, function (x) { return x.free() > 0; }, countryList.selectedCity().stations());
        var nearestArrival = nearestStations(arrival, 2, function (x) { return (x.total() == -1) || (x.total() - x.free()) > 0; }, countryList.selectedCity().stations());

        //erase the routes infobox
        var routesInfoBox = document.getElementById("routes");
        routesInfoBox.innerHTML = "";

        //for each station in departure stations/arrival stations get the route
        for (var i = 0; i < nearestDeparture.length; i++) {
            for (var j = 0; j < nearestArrival.length; j++) {
                var dep = nearestDeparture[i];
                var arr = nearestArrival[j];

                var from = new google.maps.LatLng(dep.station.lat(), dep.station.lng());
                var to = new google.maps.LatLng(arr.station.lat(), arr.station.lng());

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

        if (countryList.selectedCity() == null) {
            
            while (countryList.countries() == null) { }
            if (countryList.cities().length > 0) {
                countryList.setSelected(nearestCity(latitude, longitude, countryList.cities()));
            }
        } else {
            findAndShowNearest(currentPosition);
        }

        //we have position - we can get nearest stations later when they are loaded.
        positionFound = true;
        currentPosition = latlng;
    }

    function setMapToCurrentCity() {
        var center = new google.maps.LatLng(countryList.selectedCity().lat, countryList.selectedCity().lng);
        map.setCenter(center);
    }

    function findAndShowNearest(pos) {
        if (currentStation != null) {
            currentStation.info.close();
        }

        clearArr(lines);
        clearArr(stationsArray);
        
        nearStations = nearestStations(pos, 5, function (x) { return true; }, countryList.selectedCity().stations());
        addStationsToList('nearStations', nearStations);
        addStationsToMap(nearStations, createStationMarker);
        //$("#nearStations").toggle(true);
    }

    function createStationMarker(stationData) {
        var latlng = new google.maps.LatLng(stationData.lat(), stationData.lng());
        var image;
        
        if (stationData.free() > 2) {
            image = 'Img/station_green.png';
        }
        else if (stationData.free() <= 0) {
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
        if (stationData.total() != -1) {
            freeplaces = stationData.total() - stationData.free();
        }else{
            freeplaces = 'not determined'
        }

        
        var boxText = "<div id='boxText'><p class='address'>" + stationData.address() + "</p><p class='free'>bikes:" + stationData.free() + '</p><p class="notfree">places:' + freeplaces + "</p></div>";

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
        latestCenter = latlng;
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
             if (delta(center, latestCenter, 0.008)) {
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

        if (countryList.selectedCity() != null) {
            clearArr(stationsArray);
            nearStations = nearestStations(map.getCenter(), 50, function (x) { return true; }, countryList.selectedCity().stations());
            addStationsToMap(nearStations,createStationMarker);
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
    }


    $(document).ready(function () {
        createMap();
        addListeners();
        setClickCallbacks();

        countryList = new CountryListViewModel();

        //custom subscription to get rid of the map points
        countryList.selectedCity.subscribe(function (newValue) {

            //hide the menu
            $("a.trigger").next("ul").toggle(false);

            if (countryList.OldCity != null) {
                var oldCityTips = countryList.oldCity.tips
                for (var i = 0; i < markers.length; i++) {
                    var marker = markers[i];
                    marker.setMap(null);
                }
                markers = [];
            }
            setMapToCurrentCity();
            addNearestToCenterToMap();
        });

        $("a.trigger").click(function () {
            $(this).next("ul").toggle("fast").siblings(".menuToggle").hide("fast");
            $(this).next("div.menuToggle").toggle("fast").siblings(".menuToggle").hide("fast");
        });

        ko.applyBindings(countryList);

        getCurrentLocation();
    });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="map_canvas">
    </div>
    <nav id="menu">
        <a href="#" class="trigger">choose city</a>
        <ul data-bind="template: {name:'country-menu-template', foreach: countries,afterRender: afterCountriesRendered}" style="background-color:#808080"></ul>
        <a href="#" class="trigger" id="a_nearStations">near stations</a><ul id="nearStations" style="background-color: #808080">
        </ul>
        <a href="#" class="trigger" id="a_findRoute">find route</a><ul id="routes" style="background-color: #808080">
            <li>use your mouse to set the arrival and departure</li></ul>
    </nav>

    <!-- COUNTRY MENU TEMPLATE -->
    <script type="text/html" id="country-menu-template">
        <li class="li_country"><span data-bind="text: name"></span>
                <ul id="cityList" data-bind="foreach:cities" class="ul_city" style="display:none">
                    <li data-bind="click: $root.setSelected" style="cursor:pointer"><span data-bind="text: name"></span></li>
                </ul>
            </li>
    </script>
</asp:Content>
