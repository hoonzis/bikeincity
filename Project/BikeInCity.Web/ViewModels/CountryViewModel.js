function CountryViewModel(data) {
    // Data
    var self = this;
    self.name = data.Name;
    self.id = data.Id;
    self.cities = ko.observableArray([]);
    self.selectedCity = ko.observable();
    self.oldCity = null;

    // Load initial state from server, convert it to Task instances, then populate self.tasks
    $.getJSON("http://" + location.host + "/Services/Bike.svc/json/countries/" + self.id + "/cities", function (allData) {
        var mappedCities = $.map(allData, function (item) { return new CityViewModel(item) });
        self.cities(mappedCities);
    });
}