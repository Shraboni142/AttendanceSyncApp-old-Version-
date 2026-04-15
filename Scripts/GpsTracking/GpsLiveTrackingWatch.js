var gpsWatchId = null;
var isTrackingRunning = false;
var hasRedirectedToMap = false;

function getLiveTrackingElements() {
    return {
        trackerUserId: document.getElementById("hdnTrackerUserId"),
        mobileNo: document.getElementById("txtMobileNo"),
        employeeCode: document.getElementById("txtEmployeeCode"),
        employeeName: document.getElementById("txtEmployeeName"),
        latitude: document.getElementById("txtLatitude"),
        longitude: document.getElementById("txtLongitude"),
        accuracy: document.getElementById("txtAccuracy"),
        status: document.getElementById("trackingStatus")
    };
}

function getTrackerPayloadBase() {
    var el = getLiveTrackingElements();

    return {
        TrackerUserId: el.trackerUserId ? el.trackerUserId.value : "",
        MobileNo: el.mobileNo ? el.mobileNo.value : "",
        EmployeeCode: el.employeeCode ? el.employeeCode.value : "",
        EmployeeName: el.employeeName ? el.employeeName.value : ""
    };
}

function updateTrackingStatus(message) {
    var el = getLiveTrackingElements();
    if (el.status) {
        el.status.innerText = message;
    }
}

function saveWatchedLiveLocation(position) {
    var el = getLiveTrackingElements();
    var base = getTrackerPayloadBase();

    if (!base.TrackerUserId) {
        updateTrackingStatus("Tracker user not loaded.");
        return;
    }

    var payload = {
        TrackerUserId: base.TrackerUserId,
        MobileNo: base.MobileNo,
        EmployeeCode: base.EmployeeCode,
        EmployeeName: base.EmployeeName,
        Latitude: position.coords.latitude,
        Longitude: position.coords.longitude,
        AccuracyMeter: position.coords.accuracy
    };

    if (el.latitude) el.latitude.value = payload.Latitude;
    if (el.longitude) el.longitude.value = payload.Longitude;
    if (el.accuracy) el.accuracy.value = payload.AccuracyMeter || "";

    fetch('/GpsTracking/SaveLiveLocation', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json; charset=utf-8'
        },
        body: JSON.stringify(payload)
    })
        .then(function (response) { return response.json(); })
        .then(function (data) {
            if (data && data.success) {
                updateTrackingStatus("Live tracking running. Last sent: " + new Date().toLocaleString());

                if (isTrackingRunning && !hasRedirectedToMap) {
                    hasRedirectedToMap = true;

                    var redirectInput = document.getElementById("hdnMapRedirectUrl");
                    var redirectUrl = redirectInput && redirectInput.value
                        ? redirectInput.value
                        : "/AdminGpsSystem/LiveMap";

                    setTimeout(function () {
                        window.location.href = redirectUrl;
                    }, 1500);
                }
            } else {
                updateTrackingStatus("Location send failed.");
                console.error(data);
            }
        })
        .catch(function (error) {
            updateTrackingStatus("Server error while sending location.");
            console.error(error);
        });
}

function startLiveTrackingWatch() {
    var base = getTrackerPayloadBase();

    if (!base.TrackerUserId) {
        alert("Please load tracker user first.");
        return;
    }

    if (!navigator.geolocation) {
        alert("Geolocation is not supported by this browser.");
        return;
    }

    if (gpsWatchId !== null) {
        navigator.geolocation.clearWatch(gpsWatchId);
        gpsWatchId = null;
    }

    gpsWatchId = navigator.geolocation.watchPosition(
        function (position) {
            saveWatchedLiveLocation(position);
        },
        function (error) {
            updateTrackingStatus("GPS error: " + error.message);
            console.error(error);
        },
        {
            enableHighAccuracy: true,
            maximumAge: 0,
            timeout: 15000
        }
    );

    isTrackingRunning = true;
    updateTrackingStatus("Live tracking started...");
    alert("Live tracking started.");
}

function stopLiveTrackingWatch() {
    if (gpsWatchId !== null) {
        navigator.geolocation.clearWatch(gpsWatchId);
        gpsWatchId = null;
    }

    isTrackingRunning = false;
    updateTrackingStatus("Live tracking stopped.");
    alert("Live tracking stopped.");
}

document.addEventListener("DOMContentLoaded", function () {
    var startBtn = document.getElementById("btnStartLiveTracking");
    var stopBtn = document.getElementById("btnStopLiveTracking");

    if (startBtn) {
        startBtn.addEventListener("click", startLiveTrackingWatch);
    }

    if (stopBtn) {
        stopBtn.addEventListener("click", stopLiveTrackingWatch);
    }
});