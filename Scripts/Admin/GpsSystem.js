$(document).ready(function () {
    alert("GpsSystem.js loaded");

    loadTrackerUsers();

    $("#btnSaveTrackerUser").click(function () {
        alert("Save button clicked");
        saveTrackerUser();
    });
});

function saveTrackerUser() {
    var dto = {
        EmployeeCode: $("#txtEmployeeCode").val(),
        EmployeeName: $("#txtEmployeeName").val(),
        MobileNo: $("#txtMobileNo").val(),
        DepartmentName: $("#txtDepartmentName").val(),
        BranchName: $("#txtBranchName").val(),
        IsActive: $("#chkIsActive").is(":checked"),
        IsLiveTrackingEnabled: $("#chkIsLiveTrackingEnabled").is(":checked"),
        IsFieldVisitEnabled: $("#chkIsFieldVisitEnabled").is(":checked")
    };

    $.ajax({
        url: "/AdminGpsSystem/SaveTrackerUser",
        type: "POST",
        data: dto,
        success: function (response) {
            alert(response.message);

            if (response.success) {
                clearTrackerUserForm();
                loadTrackerUsers();
            }
        },
        error: function (xhr) {
            alert("Save failed");
            console.log(xhr.responseText);
        }
    });
}

function loadTrackerUsers() {
    $.ajax({
        url: "/AdminGpsSystem/GetTrackerUsers",
        type: "GET",
        success: function (data) {
            var html = "";

            if (!data || data.length === 0) {
                html = '<tr><td colspan="10" class="text-center">No data found.</td></tr>';
            } else {
                for (var i = 0; i < data.length; i++) {
                    html += "<tr>";
                    html += "<td>" + (i + 1) + "</td>";
                    html += "<td>" + nullToText(data[i].EmployeeCode) + "</td>";
                    html += "<td>" + nullToText(data[i].EmployeeName) + "</td>";
                    html += "<td>" + nullToText(data[i].MobileNo) + "</td>";
                    html += "<td>" + nullToText(data[i].DepartmentName) + "</td>";
                    html += "<td>" + nullToText(data[i].BranchName) + "</td>";
                    html += "<td>" + yesNoText(data[i].IsActive) + "</td>";
                    html += "<td>" + yesNoText(data[i].IsLiveTrackingEnabled) + "</td>";
                    html += "<td>" + yesNoText(data[i].IsFieldVisitEnabled) + "</td>";
                    html += "<td>" + nullToText(data[i].CreatedAtText) + "</td>";
                    html += "</tr>";
                }
            }

            $("#trackerUserTableBody").html(html);
        },
        error: function (xhr) {
            $("#trackerUserTableBody").html('<tr><td colspan="10" class="text-center">Failed to load data.</td></tr>');
            console.log(xhr.responseText);
        }
    });
}

function clearTrackerUserForm() {
    $("#txtEmployeeCode").val("");
    $("#txtEmployeeName").val("");
    $("#txtMobileNo").val("");
    $("#txtDepartmentName").val("");
    $("#txtBranchName").val("");
    $("#chkIsActive").prop("checked", true);
    $("#chkIsLiveTrackingEnabled").prop("checked", true);
    $("#chkIsFieldVisitEnabled").prop("checked", true);
}

function nullToText(value) {
    if (value === null || value === undefined) {
        return "";
    }
    return value;
}

function yesNoText(value) {
    if (value) {
        return "Yes";
    }
    return "No";
}