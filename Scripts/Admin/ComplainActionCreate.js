$(document).ready(function () {

    $("#btnSaveComplainAction").click(function () {
        var id = $("#Id").val();
        var offenceType = $("#OffenceType").val();
        var complainActionType = $("#ComplainActionType").val();
        var dateOfNotice = $("#DateOfNotice").val();

        if (!offenceType) {
            alert("Offence Type is required.");
            return;
        }

        if (!complainActionType) {
            alert("Complain Action Type is required.");
            return;
        }

        if (!dateOfNotice) {
            alert("Date of Notice is required.");
            return;
        }

        var selectedDate = new Date(dateOfNotice);
        var selectedYear = selectedDate.getFullYear();

        if (selectedYear < 1753) {
            alert("Date of Notice year must be 1753 or later.");
            return;
        }

        var earlyWithdrawalDate = $("#EarlyWithdrawalDate").val();
        if (earlyWithdrawalDate) {
            var earlyDate = new Date(earlyWithdrawalDate);
            var earlyYear = earlyDate.getFullYear();

            if (earlyYear < 1753) {
                alert("Early Withdrawal Date year must be 1753 or later.");
                return;
            }
        }
        // ADD END: Date validation

        var attachmentInput = $("#AttachmentFile")[0];
        if (attachmentInput && attachmentInput.files.length > 0) {
            var maxSizeInBytes = 2 * 1024 * 1024; // 2 MB
            if (attachmentInput.files[0].size > maxSizeInBytes) {
                alert("File size must be 2 MB or less.");
                return;
            }
        }

        if (!$("#ReviewStatus").val()) {
            $("#ReviewStatus").val("Pending");
        }

        var form = $("#ComplainActionForm")[0];
        var formData = new FormData(form);

        var url = '/AdminComplainAction/SaveComplainAction';

        if (id && parseInt(id) > 0) {
            url = '/AdminComplainAction/UpdateComplainAction';
        }

        $.ajax({
            url: url,
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                if (response.success) {
                    alert(response.message);
                    window.location.href = '/AdminComplainAction/Index';
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