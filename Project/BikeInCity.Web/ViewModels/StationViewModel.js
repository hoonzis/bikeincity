function StationViewModel(cityId, parent) {
    var self = this;
    self.parent = parent;
    self.cityId = cityId;
    self.address = ko.observable();
    self.lat = ko.observable();
    self.lng = ko.observable();
}