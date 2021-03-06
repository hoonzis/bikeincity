﻿function CityViewModel(data) {
    var self = this;

    self.name = ko.observable(data.Name);
    self.id = data.Id;
    self.tips = ko.observableArray([]);
    self.stations = ko.observableArray([]);

    self.newTip = ko.observable(new TipViewModel(self.id, self));

    self.saving = ko.observable();
    self.selectedTip = ko.observable();
    self.oldTip = null;
    self.lat = data.Lat;
    self.lng = data.Lng;
    self.stationsLoaded = ko.observable();

    self.description = ko.observable();
    self.stationImageUrl = ko.observable();
    self.bikeImageUrl = ko.observable();

    self.getTips = function () {
        $.getJSON("/Services/Info.svc/json/city/" + this.id + "/tips", function (allData) {
            var mappedTips = $.map(allData, function (item) {
                var tip = new TipViewModel(self.id, self);
                tip.title(item.Title);
                tip.description(item.Description);
                tip.imageUrl(item.ImageUrl);
                tip.lng(item.Lng);
                tip.lat(item.Lat);
                return tip;
            });
            self.tips(mappedTips);
            self.stationsLoaded(true);
        });
    }

    self.getStations = function () {
        $.ajax({
            type: 'GET',
            url: "http://" + location.host + "/Services/Bike.svc/json/city/" + this.id + "/stations",
            dataType: 'json',
            success: function (allData) {
                var mappedStations = $.map(allData, function (item) {
                    var station = new StationViewModel(self.id, self);
                    station.address(item.Address);
                    station.lng(item.Lng);
                    station.lat(item.Lat);
                    station.free(item.Free);
                    station.total(item.Total);
                    return station;
                });
                self.stations(mappedStations);
            },
            async: false
        });

    };

    // Operations
    self.addTip = function () {
        self.newTip().save();
        self.tips.push(self.newTip());
        self.newTip(new TipViewModel(self.id, self));
    };

    self.save = function () {
        self.saving(true);

        var toSend = new Object();
        toSend.Name = self.name;
        toSend.Description = self.description;
        toSend.Id = self.id;

        $.ajax("/Services/Info.svc/json/cities/update", {
            data: toSend,
            type: "post", contentType: "application/json",
            success: function (result) {
                self.saving(false);
            }
        });
    }
}