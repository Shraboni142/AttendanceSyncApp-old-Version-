$(document).ready(function () {

    $(document).on("click", ".btnDeleteComplianceAction", function () {
        var id = $(this).data("id");

        if (confirm("Are you sure you want to delete this record?")) {
            $.ajax({
                url: '/AdminComplianceAction/DeleteComplianceAction',
                type: 'POST',
                data: { id: id },
                success: function (response) {
                    if (response.success) {
                        alert(response.message);
                        window.location.href = '/AdminComplianceAction/Index';
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

    $(document).on("change", ".reviewStatusDropdown", function () {
        var id = $(this).data("id");
        var reviewStatus = $(this).val();

        $.ajax({
            url: '/Admin