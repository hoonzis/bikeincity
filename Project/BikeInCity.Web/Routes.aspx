<%@ Page Language="C#" MasterPageFile="~/BikeMaster.Master" AutoEventWireup="true" CodeBehind="Routes.aspx.cs" Inherits="BikeInCity.Web.Routes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="server">
    <link href="css/informations-styles.css" rel="stylesheet" type="text/css" />
    <link href="css/ui-lightness/jquery-ui-1.8.14.custom.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="http://maps.googleapis.com/maps/api/js?sensor=true"></script>
    <script src="/Scripts/knockout-2.0.0.debug.js" type="text/javascript"></script>
    <script src="/Scripts/knockout.mapping-latest.debug.js" type="text/javascript"></script>
    <script src="/Scripts/jquery-1.7.1.js" type="text/javascript"></script>
    <script src="/Scripts/jquery-ui-1.8.17.js" type="text/javascript"></script>
    <!-- ViewModels -->
    <script src="ViewModels/CityViewModel.js" type="text/javascript"></script>
    <script src="ViewModels/CountryViewModel.js" type="text/javascript"></script>
    <script src="ViewModels/TipViewModel.js" type="text/javascript"></script>
    <script src="ViewModels/StationViewModel.js" type="text/javascript"></script>
    <script src="ViewModels/CountryListViewModel.js" type="text/javascript"></script>
    <script src="Scripts/biking.js" type="text/javascript"></script>
    <script type="text/javascript">
        //globals
        var map;
        var stationMarkers = [];
        var countryList;
        var markers = [];

        function createMap() {
            var latlng = new google.maps.LatLng(0.0, 0.0);
            var myOptions =
            {
                zoom: 11,
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
            google.maps.event.addListener(map, 'click', function (event) {
                if (currentPoint != null) {
                    newPoint = event.latLng;
                    points.put(newPoint);
                    showLine(currentPoint, newPoint);
                    newPoint = currentPoint;
                }
            });
        }

        

        $(document).ready(function () {
        

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

                var center = new google.maps.LatLng(countryList.selectedCity().lat, countryList.selectedCity().lng);
                map.setCenter(center);

            });


            createMap();
            
            //Hide (Collapse) the toggle containers on load
            //$(".menuToggle").hide();

            //Switch the "Open" and "Close" state per click then slide up/down (depending on open/close state)
            $("a.trigger").click(function () {
                $(this).next("ul").toggle("fast").siblings(".menuToggle").hide("fast");
                $(this).next("div.menuToggle").toggle("fast").siblings(".menuToggle").hide("fast");
            });

            ko.applyBindings(countryList);
        });

        function imageUrlUploadSwitch() {
            var enableUpload = $("#enableUpload").attr("checked") == "checked";
            $("#uploadForm").toggle(enableUpload);
            $("#providedUrl").toggle(!enableUpload);

            if (enableUpload) {
                $("#uploadForm").attr("disabled", false);
                $("#providedUrl").attr("disabled", true);
            } else {
                $("#uploadForm").attr("disabled", true);
                $("#providedUrl").attr("disabled", false);
            }
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
            var latlng = new google.maps.LatLng(stationData.lat(), stationData.lng());
            var image = 'Img/small_cycler.png';

            var marker = new google.maps.Marker({
                position: latlng,
                map: map,
                icon: image
            });
            stationMarkers.push(marker);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="tips_column">
        <table style="background-color: #F7B54A; color: White; width: 100%; height: 5%">
            <tr>
                <td>
                    <div data-bind="text: selectedCity()? selectedCity().name : 'Select the city'" />
                </td>
                <td>
                    <div data-bind="visible: selectedCity()" id="dialog_link" style="background-color: #808080;
                        border-radius: 4px; cursor: pointer; float: right">
                        <span style="margin: 5px">add new</span>
                    </div>
                </td>
            </tr>
        </table>
        <!-- ko if:selectedCity() -->
        <!-- map points -->
        <div data-bind="foreach: selectedCity().tips">
            <div data-bind="latitude: lat, longitude:lng, map:map, selected:selected">
            </div>
        </div>
        <div class="tips_table">
            <table data-bind="foreach: selectedCity().tips" style="width: 100%">
                <tr style="cursor: pointer;">
                    <td data-bind="click: select,style: { backgroundColor: selected() ? '#66C547' : '#808080'}">
                        <img data-bind='attr: { src: imageUrl }' height="60px" width="60px" style="float: left" />
                        <div style="float: left; color: White; margin: 4px">
                            <div data-bind='text: title' style="font-weight: bold">
                            </div>
                            <div data-bind="text: description">
                            </div>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
        <!-- /ko -->
    </div>
    <!-- The MAP -->
    <div id="map_canvas">
    </div>
    <!-- THE NEW TIP POPUP -->
    <div id="newRoute" style="background-color: Gray; color: White" title="New bike target"
        data-bind="with:selectedCity()">
        <table>
            <tr>
                <td>
                    Title:
                </td>
                <td>
                    <input data-bind="value: $data ? newTip().title : '' " />
                </td>
            </tr>
            <tr>
                <td>
                    Description:
                </td>
                <td>
                    <textarea data-bind="value: $data? newTip().description : ''" rows='3' cols='40'></textarea>
                </td>
            </tr>
            <tr style="height:100px;margin-bottom:4px">
                <td>
                    Image URL:
                </td>
                <td>
                    <input data-bind="value: $data? newTip().imageUrl : ''" id="providedUrl" />
                </td>
            </tr>
        </table>
        <button data-bind='click: $data? addTip : new function(){}'>Add route</button>
    </div>
    
    <!-- COUNTRY MENU TEMPLATE -->
    <script type="text/html" id="country-menu-template">
        <li class="li_country"><span data-bind="text: name"></span>
                <ul id="cityList" data-bind="foreach:cities" class="ul_city" style="display:none">
                    <li data-bind="click: $root.setSelected" style="cursor:pointer"><span data-bind="text: name"></span></li>
                </ul>
            </li>
    </script>
    <!-- THE MENU -->
    <nav id="menu">
        <a href="#" class="trigger">choose city</a>
        <ul data-bind="template: {name:'country-menu-template', foreach: countries,afterRender: afterCountriesRendered}" style="background-color:#808080"></ul>
    </nav>
</asp:Content>

