$(document).ready(function () {

    $("#btnSaveComplianceAction").click(function () {
        var offenceType = $("#OffenceType").val();
        var complianceActionType = $("#ComplianceActionType").val();
        var dateOfNotice = $("#DateOfNotice").val();

        if (!offenceType) {
            alert("Offence Type is required.");
            return;
        }

        if (!complianceActionType) {
            alert("Compliance Action Type is required.");
            return;
        }

        if (!dateOfNotice) {
            alert("Date of Notice is required.");
            return;
        }

        var form = $("#complianceActionForm")[0];
        var formData = new FormData(form);

        $.ajax({
            url: '/AdminComplianceAction/SaveComplianceAction',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                if (response.success) {
                    alert(response.message);
                    window.location.href = '/AdminComplianceAction/Index';
                } else {
                    alert(response.message);
                }
            },
            error: function () {
                alert("An error occurred while saving.");
            }
        });
    });

});