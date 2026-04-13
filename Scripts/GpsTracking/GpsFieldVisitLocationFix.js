(function () {
    function getElement(id) {
        return document.getElementById(id);
    }

    function setValue(id, value) {
        var el = getElement(id);
        if (el) {
            el.value = value || "";
        }
    }

    function clearLocationFields() {
        setValue("txtLatitude", "");
        setValue("txtLongitude", "");
        setValue("txtAccuracy", "");
    }

    function setLocationFields(position) {
        setValue("txtLatitude", position.coords.latitude);
        setValue("txtLongitude", position.coords.longitude);
        setValue("txtAccuracy", position.coords.accuracy || "");
    }

    function setButtonLoading(isLoading) {
        var btn = getElement("btnGetLocation");
        if (!btn) return;

        if (isLoading) {
            btn.disabled = true;
            btn.setAttribute("data-old-text", btn.innerHTML);
            btn.innerHTML = "Getting Location...";
        } else {
            btn.disabled = false;
            var oldText = btn.getAttribute("data-old-text");
            btn.innerHTML = oldText || "Get Current Location";
        }
    }

    function showLocationHelpMessage(message) {
        alert(message);
    }

    function getDetailedErrorMessage(error) {
        if (!error) {
            return "Unable to get current location. Unknown error occurred.";
        }

        switch (error.code) {
            case 1:
                return "Location permission denied. Please allow browser location permission, turn on Windows location service, then try again.";
            case 2:
                return "Location information is unavailable. Your laptop may not have GPS or Windows/browser could not detect position.";
            case 3:
                return "Location request timed out. Please move to an open area, ensure internet/location is on, then try again.";
            default:
                return "Failed to get current location. " + (error.message || "Unknown error.");
        }
    }

    function validateLocationValues() {
        var lat = (getElement("txtLatitude") || {}).value || "";
        var lng = (getElement("txtLongitude") || {}).value || "";

        if (!lat || !lng) {
            return false;
        }

        var latNum = parseFloat(lat);
        var lngNum = parseFloat(lng);

        if (isNaN(latNum) || isNaN(lngNum)) {
            return false;
        }

        if (latNum < -90 || latNum > 90) {
            return false;
        }

        if (lngNum < -180 || lngNum > 180) {
            return false;
        }

        return true;
    }

    function requestCurrentLocation(options, onSuccess, onFail) {
        navigator.geolocation.getCurrentPosition(onSuccess, onFail, options);
    }

    window.getCurrentLocation = function () {
        if (!window.isSecureContext) {
            showLocationHelpMessage("Location access works only in secure context. Please use HTTPS or localhost.");
            return;
        }

        if (!navigator.geolocation) {
            showLocationHelpMessage("Geolocation is not supported by this browser.");
            return;
        }

        clearLocationFields();
        setButtonLoading(true);

        requestCurrentLocation(
            {
                enableHighAccuracy: true,
                timeout: 20000,
                maximumAge: 0
            },
            function (position) {
                setLocationFields(position);
                setButtonLoading(false);
                showLocationHelpMessage("Current location captured successfully.");
            },
            function (error) {
                requestCurrentLocation(
                    {
                        enableHighAccuracy: false,
                        timeout: 15000,
                        maximumAge: 60000
                    },
                    function (position) {
                        setLocationFields(position);
                        setButtonLoading(false);
                        showLocationHelpMessage("Current location captured successfully (approximate mode).");
                    },
                    function (fallbackError) {
                        setButtonLoading(false);

                        var finalMessage =
                            getDetailedErrorMessage(error) +
                            "\n\nFallback attempt also failed." +
                            "\n\nPlease check:\n1. Browser location permission = Allow\n2. Windows Location = On\n3. Laptop internet/Wi-Fi is active\n4. Try from mobile phone for accurate field GPS";

                        console.error("Primary geolocation error:", error);
                        console.error("Fallback geolocation error:", fallbackError);

                        showLocationHelpMessage(finalMessage);
                    }
                );
            }
        );
    };

    window.saveFieldVisit = function () {
        var trackerUserId = (getElement("hdnTrackerUserId") || {}).value || "";
        var mobileNo = (getElement("txtMobileNo") || {}).value || "";
        var employeeCode = (getElement("txtEmployeeCode") || {}).value || "";
        var employeeName = (getElement("txtEmployeeName") || {}).value || "";
        var visitPurpose = (getElement("ddlVisitPurpose") || {}).value || "";
        var clientName = (getElement("txtClientName") || {}).value || "";
        var collectionNo = (getElement("txtCollectionNo") || {}).value || "";
        var remarks = (getElement("txtRemarks") || {}).value || "";
        var latitude = (getElement("txtLatitude") || {}).value || "";
        var longitude = (getElement("txtLongitude") || {}).value || "";
        var accuracy = (getElement("txtAccuracy") || {}).value || "";

        if (!trackerUserId) {
            alert("Please load tracker user first.");
            return;
        }

        if (!mobileNo) {
            alert("Mobile No is required.");
            return;
        }

        if (!validateLocationValues()) {
            alert("Valid current location is required before save. Please click 'Get Current Location' again.");
            return;
        }

        var body =
            "TrackerUserId=" + encodeURIComponent(trackerUserId) +
            "&MobileNo=" + encodeURIComponent(mobileNo) +
            "&EmployeeCode=" + encodeURIComponent(employeeCode) +
            "&EmployeeName=" + encodeURIComponent(employeeName) +
            "&VisitPurpose=" + encodeURIComponent(visitPurpose) +
            "&ClientName=" + encodeURIComponent(clientName) +
            "&CollectionNo=" + encodeURIComponent(collectionNo) +
            "&Remarks=" + encodeURIComponent(remarks) +
            "&Latitude=" + encodeURIComponent(latitude) +
            "&Longitude=" + encodeURIComponent(longitude) +
            "&AccuracyMeter=" + encodeURIComponent(accuracy);

        var xhr = new XMLHttpRequest();
        xhr.open("POST", "/GpsTracking/SaveFieldVisit", true);
        xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");

        xhr.onreadystatechange = function () {
            if (xhr.readyState === 4) {
                if (xhr.status === 200) {
                    var response = JSON.parse(xhr.responseText);
                    alert(response.message);
                } else {
                    alert("Failed to save field visit.");
                }
            }
        };

        xhr.send(body);
    };
})();