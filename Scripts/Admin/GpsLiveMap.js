var liveMap;
var liveMarkers = [];

function initLiveMap() {
    liveMap = L.map('liveMap').setView([23.777176, 90.399452], 12);

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; OpenStreetMap contributors'
    }).addTo(liveMap);

    loadLiveLocations();
    setInterval(loadLiveLocations, 10000);
}

function clearLiveMarkers() {
    for (var i = 0; i < liveMarkers.length; i++) {
        liveMap.removeLayer(liveMarkers[i]);
    }
    liveMarkers = [];
}

function loadLiveLocations() {
    fetch('/GpsTracking/GetCurrentLiveLocations')
        .then(response => response.json())
        .then(result => {
            if (!result.success) {
                console.error("Failed to load live locations.");
                return;
            }

            clearLiveMarkers();

            var data = result.data || [];
            document.getElementById("liveTrackerCount").innerText = "Active Trackers: " + data.length;

            data.forEach(function (item) {
                var marker = L.marker([item.Latitude, item.Longitude]).addTo(liveMap);

                marker.bindPopup(
                    "<b>" + (item.EmployeeName || "") + "</b><br/>" +
                    "Code: " + (item.EmployeeCode || "") + "<br/>" +
                    "Mobile: " + (item.MobileNo || "") + "<br/>" +
                    "Accuracy: " + (item.AccuracyMeter || "") + "<br/>" +
                    "Time: " + (item.EntryTime || "")
                );

                liveMarkers.push(marker);
            });

            if (data.length > 0) {
                liveMap.setView([data[0].Latitude, data[0].Longitude], 14);
            }
        })
        .catch(error => {
            console.error("Live map error:", error);
        });
}

document.addEventListener("DOMContentLoaded", function () {
    if (document.getElementById("liveMap")) {
        initLiveMap();
    }
});