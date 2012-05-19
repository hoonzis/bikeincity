function calculateDistance(lat1, lat2, lon1, lon2) {
    var R = 6371000; // km
    var dLat = toRad((lat2 - lat1));
    var dLon = toRad((lon2 - lon1));
    var lat1 = toRad(lat1);
    var lat2 = toRad(lat2);

    var a = Math.sin(dLat / 2) * Math.sin(dLat / 2) +
            Math.sin(dLon / 2) * Math.sin(dLon / 2) * Math.cos(lat1) * Math.cos(lat2);
    var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
    var d = R * c;

    return d;
}

function toRad(value) {

      var r = value * 3.14159 / 180;
      return r;
}

function computeDistance(station, pos) {
    var value = calculateDistance(station.Lat, pos.lat(), station.Lng, pos.lng());
    return value;
}

function nearestStations(pos, count, predict, stationArray) {
    if (stationArray == null) { return []; }

    var stationsCopy = [];
    for (var i = 0; i < stationArray.length; i++) {
        var station = stationArray[i];
        if (predict(station)) {
            var staDist = new Object();
            staDist.dist = calculateDistance(station.Lat, pos.lat(), station.Lng, pos.lng());
            staDist.station = station;
            stationsCopy.push(staDist);
        }
    }
    var sorted = stationsCopy.sort(function (x, y) { return x.dist - y.dist; });

    var result = [];
    for (i = 0; i < count && i< sorted.length; i++) {
        result.push(sorted[i]);
    }
    return result;
}

function nearestCity(latitude,longitude, citiArray) {
    var minDist = 10000000;
    var minCity;
    for (var i = 0; i < citiArray.length; i++) {
        bcity = citiArray[i];
        dist = calculateDistance(bcity.Lat, latitude, bcity.Lng, longitude);
        if (dist < minDist) {
            minDist = dist;
            minCity = bcity;
        }
    }
    return minCity;
}

function clearArr(arr) {
    if (arr) {
        for (i in arr) {
            arr[i].setMap(null);
        }
        arr.length = 0;
    }
}

function findCity(id, cityList) {
    for (k = 0; k < cityList.length; ++k) {
        if (cityList[k].Id == id) {
            return cityList[k];
        }
    }
}

function directLine(start, end) {
    var coordinates = [start, end, ];
    var routePath = new google.maps.Polyline({
        path: coordinates,
        strokeColor: "#FF0000",
        strokeOpacity: 1.0,
        strokeWeight: 2,
        clickable: false
    });

    return routePath;
}

function delta(a, b, threshold) {
    return Math.abs(a.lat() - b.lat()) > threshold || Math.abs(a.lng() - b.lng()) > threshold;
}

function nearestStationsVM(lat,lng, count, stationArray) {
    if (stationArray == null) { return []; }

    var stationsCopy = [];
    for (var i = 0; i < stationArray.length; i++) {
        var station = stationArray[i];
        var staDist = new Object();
        staDist.dist = calculateDistance(station.lat(), lat, station.lng(), lng);
        staDist.station = station;
        stationsCopy.push(staDist);
    }
    var sorted = stationsCopy.sort(function (x, y) { return x.dist - y.dist; });

    var result = [];
    for (i = 0; i < count && i < sorted.length; i++) {
        result.push(sorted[i]);
    }
    return result;
}