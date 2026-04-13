var gpsLiveTrackingInterval = null;

function getTrackerLiveData() {
    return {
        TrackerUserId: document.getElementById("hdnTrackerUserId") ? document.getElementById("hdnTrackerUserId").value : "",
        MobileNo: document.getElementById("txtMobileNo") ? document.getElementById("txtMobileNo").value : "",
        EmployeeCode: document.getElementById("txtEmployeeCode") ? document.getElementById("txtEmployeeCode").value : "",
        EmployeeName: document.getElementById("txtEmployeeName") ? document.getElementById("txtEmployeeName").value : ""
    };
}

function saveCurrentLiveLocation() {
    var tracker = getTrackerLiveData();

    if (!tracker.TrackerUserId) {
        console.log("Tracker user not loaded.");
        return;
    }

    if (!navigator.geolocation) {
        alert("Geolocation is not supported by this browser.");
        return;
    }

    navigator.geolocation.getCurrentPosition(function (position) {
        var payload = {
            TrackerUserId: tracker.TrackerUserId,
            MobileNo: tracker.MobileNo,
            EmployeeCode: tracker.EmployeeCode,
            EmployeeName: tracker.EmployeeName,
            Latitude: position.coords.latitude,
            Longitude: position.coords.longitude,
            AccuracyMeter: position.coords.accuracy
        };

        fetch('/GpsTracking/SaveLiveLocation', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(payload)
        })
            .then(response => response.json())
            .then(data => {
                console.log("Live location response:", data);
            })
            .catch(error => {
                console.error("Live tracking save failed:", error);
            });

    }, function (error) {
        console.error("Live tracking geolocation error:", error);
    }, {
        enableHighAccuracy: true,
        timeout: 15000,
        maximumAge: 0
    });
}

function startLiveTracking() {
    var tracker = getTrackerLiveData();

    if (!tracker.TrackerUserId) {
        alert("Please load tracker user first.");
        return;
    }

    if (gpsLiveTrackingInterval) {
        clearInterval(gpsLiveTrackingInterval);
    }

    saveCurrentLiveLocation();
    gpsLiveTrackingInterval = setInterval(saveCurrentLiveLocation, 15000);

    alert("Live tracking started.");
}

function stopLiveTracking() {
    if (gpsLiveTrackingInterval) {
        clearInterval(gpsLiveTrackingInterval);
        gpsLiveTrackingInterval = null;
    }

    alert("Live tracking stopped.");
}

document.addEventListener("DOMContentLoaded", function () {
    var startBtn = document.getElementById("btnStartLiveTracking");
    var stopBtn = document.getElementById("btnStopLiveTracking");

    if (startBtn) {
        startBtn.addEventListener("click", startLiveTracking);
    }

    if (stopBtn) {
        stopBtn.addEventListener("click", stopLiveTracking);
    }
});