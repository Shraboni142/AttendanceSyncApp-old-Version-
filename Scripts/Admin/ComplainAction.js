$(document).ready(function () {

    $(document).on("click", ".btnDeleteComplainAction", function () {
        var id = $(this).data("id");

        if (confirm("Are you sure you want to delete this record?")) {
            $.ajax({
                url: '/AdminComplainAction/DeleteComplainAction',
                type: 'POST',
                data: { id: id },
                success: function (response) {
                    if (response.success) {
                        alert(response.message);
                        window.location.href = '/AdminComplainAction/Index';
                    } else {
                        alert(response.message);
                    }
                },
                error: function () {
                    alert("An error occurred while deleting.");
                }
            });
        }
    });

    $("#btnUpdateReviewStatusFromView").click(function () {
        var id = $("#ComplainActionId").val();
        var reviewStatus = $("#ReviewStatusDropdown").val();

        if (!reviewStatus) {
            alert("Please select a review status.");
            return;
        }

        $.ajax({
            url: '/AdminComplainAction/UpdateComplainReviewStatus',
            type: 'POST',
            data: { id: id, reviewStatus: reviewStatus },
            success: function (response) {
                if (response.success) {
                    $("#CurrentReviewStatus").val(reviewStatus);
                    alert(response.message);
                    location.reload();
                } else {
                    alert(response.message);
                }
            },
            error: function () {
                alert("An error occurred while updating review status.");
            }
        });
    });

    $("#btnSaveComplainAction").click(function () {
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

        var reviewStatus = $("#ReviewStatus").val();
        if (!reviewStatus) {
            $("#ReviewStatus").val("Pending");
        }

        var form = $("#ComplainActionForm")[0];
        var formData = new FormData(form);

        $.ajax({
            url: '/AdminComplainAction/SaveComplainAction',
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