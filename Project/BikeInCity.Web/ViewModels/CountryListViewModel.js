function CountryListViewModel() {
    var self = this;
    self.countries = ko.observableArray([]);
    self._cities = [];
    self.selectedCity = ko.observable();
    self.oldCity = null;
    self.stationsLoaded = ko.observable();

    self.setSelected = function (city) {
        self.oldCity = self.selectedCity();
        self.selectedCity(city);

        self.selectedCity().stationsLoaded.subscribe(function (newValue) {
            self.stationsLoaded(true);
        });

        self.selectedCity().getStations();
        self.selectedCity().getTips();
    };

    self.cities = function () {
        //Flattening arrays in JS does not really work as expected...
        //return self.countries().map(citiesForCountry);
        if (self._cities.length == 0) {
            var count = self.countries().length;
            for (var i = 0; i < count; i++) {
                var country = self.countries()[i];
                var cityCount = country.cities().length;
                for (var j = 0; j < cityCount; j++) {
                    self._cities.push(country.cities()[j]);
                }
            }
        }
        return self._cities;
    };

    $.ajax({
        type: 'GET',
        url: "http://" + location.host + "/Services/Bike.svc/json/countries",
        dataType: 'json',
        success: function (allData) {
            var mappedCountries = $.map(allData, function (item) { return new CountryViewModel(item) });
            self.countries(mappedCountries);
        },
        async: false
    });

    self.afterCountriesRendered = function (elements) {
        $(elements[1]).mouseenter(function () {
            $(this).find("ul").slideToggle("fast");
        });

        $(elements[1]).mouseleave(function () {
            $(this).find("ul").hide();
        });

        $(elements[1]).find("city").hide();
    };
}

function citiesForCountry(country) {
    return country.cities();
}