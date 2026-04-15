var gpsLiveTrackingInterval = null;

function setTrackingStatus(message, cssClass) {
    var statusBox = document.getElementById("trackingStatus");
    if (!statusBox) return;

    statusBox.className = "alert " + cssClass;
    statusBox.innerText = message;
}

function getTrackerLiveData() {
    return {
        TrackerUserId: document.getElementById("hdnTrackerUserId") ? document.getElementById("hdnTrackerUserId").value : "",
        MobileNo: document.getElementById("txtMobileNo") ? document.getElementById("txtMobileNo").value : "",
        EmployeeCode: document.getElementById("txtEmployeeCode") ? document.getElementById("txtEmployeeCode").value : "",
        EmployeeName: document.getElementById("txtEmployeeName") ? document.getElementById("txtEmployeeName").value : ""
    };
}

function loadTrackerUser() {
    var mobileNo = document.getElementById("txtMobileNo").value;

    if (!mobileNo) {
        alert("Please enter mobile number.");
        return;
    }

    fetch('/GpsTracking/GetTrackerUserByMobileNo?mobileNo=' + encodeURIComponent(mobileNo))
        .then(response => response.json())
        .then(data => {
            if (!data) {
                alert("No active tracker user found for this mobile number.");
                return;
            }

            document.getElementById("hdnTrackerUserId").value = data.Id || "";
            document.getElementById("txtEmployeeCode").value = data.EmployeeCode || "";
            document.getElementById("txtEmployeeName").value = data.EmployeeName || "";
            document.getElementById("txtBranchName").value = data.BranchName || "";

            setTrackingStatus("Tracker user loaded. Ready to start live tracking.", "alert-info");
        })
        .catch(error => {
            console.error(error);
            alert("Failed to load tracker user.");
        });
}

function saveCurrentLiveLocation() {
    var tracker = getTrackerLiveData();

    if (!tracker.TrackerUserId) {
        setTrackingStatus("Tracker user not loaded.", "alert-warning");
        return;
    }

    if (!navigator.geolocation) {
        alert("Geolocation is not supported by this browser.");
        return;
    }

    navigator.geolocation.getCurrentPosition(
        function (position) {
            document.getElementById("txtCurrentLatitude").value = position.coords.latitude;
            document.getElementById("txtCurrentLongitude").value = position.coords.longitude;
            document.getElementById("txtCurrentAccuracy").value = position.coords.accuracy || "";

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
                    if (data && data.success) {
                        setTrackingStatus("Live tracking running. Location sent successfully.", "alert-success");
                    } else {
                        setTrackingStatus((data && data.message) ? data.message : "Failed to save live location.", "alert-danger");
                    }
                })
                .catch(error => {
                    console.error(error);
                    setTrackingStatus("Live location save failed.", "alert-danger");
                });
        },
        function (error) {
            console.error(error);
            setTrackingStatus("Failed to get current location. Please allow location permission.", "alert-danger");
        },
        {
            enableHighAccuracy: true,
            timeout: 15000,
            maximumAge: 0
        }
    );
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

    setTrackingStatus("Live tracking started.", "alert-primary");
}

function stopLiveTracking() {
    if (gpsLiveTrackingInterval) {
        clearInterval(gpsLiveTrackingInterval);
        gpsLiveTrackingInterval = null;
    }

    setTrackingStatus("Live tracking stopped.", "alert-secondary");
}

document.addEventListener("DOMContentLoaded", function () {
    var loadBtn = document.getElementById("btnLoadTracker");
    var startBtn = document.getElementById("btnStartLiveTracking");
    var stopBtn = document.getElementById("btnStopLiveTracking");

    if (loadBtn) {
        loadBtn.addEventListener("click", loadTrackerUser);
    }

    if (startBtn) {
        startBtn.addEventListener("click", startLiveTracking);
    }

    if (stopBtn) {
        stopBtn.addEventListener("click", stopLiveTracking);
    }
});