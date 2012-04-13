function TipViewModel(cityId, parent) {
    var self = this;
    self.parent = parent;
    self.cityId = cityId;
    self.title = ko.observable();
    self.description = ko.observable();
    self.imageUrl = ko.observable();
    self.saving = ko.observable(false);
    self.lat = ko.observable();
    self.lng = ko.observable();
    self.selected = ko.observable(false);

    self.select = function () {
        if (self.parent.selectedTip() != null) {
            self.parent.oldTip = self.parent.selectedTip();
            self.parent.oldTip.selected(false);
        }

        self.parent.selectedTip(self);
        self.selected(true);
    };

    self.save = function () {
        self.saving(true);
        var data = JSON.stringify(self.toDTO());
        $.ajax("/Services/Info.svc/json/tips/add", {
            data: data,
            type: "post", contentType: "application/json",
            success: function (result) {
                self.saving(false);
            }
        });
    };


    self.toDTO = function () {
        var dto = new Object();
        dto.CityId = self.cityId;
        dto.Description = self.description();
        dto.Title = self.title();
        dto.ImageUrl = self.imageUrl();
        dto.Lat = self.lat();
        dto.Lng = self.lng();
        return dto;
    };
}