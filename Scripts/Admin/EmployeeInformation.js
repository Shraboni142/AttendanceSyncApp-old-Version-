let educationDropdown = [];
let educationFieldDropdowns = {
    Groups: [],
    Boards: [],
    AcademicYears: [],
    AcademicInstitutes: [],
    Divisions: [],
    Results: []
};

$(document).ready(function () {
    loadEmployees();
    loadEducationDropdown();
    loadEducationFieldDropdowns();
    loadDesignations();
    loadDepartments();

    $('#EmployeeCode').on('change', function () {
        var code = $(this).val();
        if (!code) return;

        $('#EmployeeName').val(code);
        loadEmployeeGeneralInfo(code);
        loadEmployeeAddressInfo(code);
    });

    $('#EmployeeName').on('change', function () {
        var code = $(this).val();
        if (!code) return;

        $('#EmployeeCode').val(code);
        loadEmployeeGeneralInfo(code);
        loadEmployeeAddressInfo(code);
    });

    $('#btnSaveGeneral').on('click', function () {
        saveGeneralInfo();
    });

    $('#btnSaveAddress').on('click', function () {
        saveAddressInfo();
    });

    $('#btnAddEducationRow').on('click', function () {
        addEducationRow();
    });

    $('#SameAsPresent').on('change', function () {
        if ($(this).is(':checked')) {
            $('#PermanentHouseVillageName').val($('#PresentHouseVillageName').val());
            $('#PermanentHouseNo').val($('#PresentHouseNo').val());
            $('#PermanentRoadNo').val($('#PresentRoadNo').val());
            $('#PermanentBlock').val($('#PresentBlock').val());
            $('#PermanentArea').val($('#PresentArea').val());
            $('#PermanentSector').val($('#PresentSector').val());
            $('#PermanentCountry').val($('#PresentCountry').val());
            $('#PermanentDivision').val($('#PresentDivision').val());
            $('#PermanentDistrict').val($('#PresentDistrict').val());
            $('#PermanentThanaUpazilla').val($('#PresentThanaUpazilla').val());
            $('#PermanentPostOffice').val($('#PresentPostOffice').val());
        }
    });
});

function loadEmployees() {
    $.get('/AdminEmployeeInformation/GetEmployees', function (data) {
        $('#EmployeeCode').empty().append('<option value="">-- Select Employee Code --</option>');
        $('#EmployeeName').empty().append('<option value="">-- Select Employee Name --</option>');

        $.each(data, function (i, item) {
            $('#EmployeeCode').append('<option value="' + item.EmployeeCode + '">' + item.EmployeeCode + '</option>');
            $('#EmployeeName').append('<option value="' + item.EmployeeCode + '">' + item.EmployeeName + '</option>');
        });
    });
}

function loadEducationDropdown() {
    $.get('/AdminEmployeeInformation/GetEducationDropdown', function (data) {
        educationDropdown = data || [];
    });
}

function loadEducationFieldDropdowns() {
    $.get('/AdminEmployeeInformation/GetEducationFieldDropdowns', function (data) {
        educationFieldDropdowns = data || educationFieldDropdowns;
    });
}
function loadDesignations() {
    $.get('/AdminEmployeeInformation/GetDesignations', function (data) {
        var $dropdown = $('#DesignationName');
        $dropdown.empty();
        $dropdown.append('<option value="">-- Select Designation --</option>');

        $.each(data, function (i, item) {
            $dropdown.append('<option value="' + item.Name + '">' + item.Name + '</option>');
        });
    });
}

function loadDepartments() {
    $.get('/AdminEmployeeInformation/GetDepartments', function (data) {
        var $dropdown = $('#DepartmentName');
        $dropdown.empty();
        $dropdown.append('<option value="">-- Select Department --</option>');

        $.each(data, function (i, item) {
            $dropdown.append('<option value="' + item.Name + '">' + item.Name + '</option>');
        });
    });
}
function loadEmployeeGeneralInfo(code) {
    $.get('/AdminEmployeeInformation/GetEmployeeGeneralInfo', { employeeCode: code }, function (data) {
        if (!data) return;

        $('#EmployeeDbId').val(data.Id);
        $('#EmployeeCode').val(data.EmployeeCode);
        $('#EmployeeName').val(data.EmployeeCode);

        $('#DesignationName').val(data.DesignationName);
        $('#FatherName').val(data.FatherName);
        $('#MotherName').val(data.MotherName);
        $('#MobileNo').val(data.MobileNo);
        $('#DepartmentName').val(data.DepartmentName);
        $('#BranchName').val(data.BranchName);
        $('#BasicSalary').val(data.BasicSalary);
        $('#DateOfBirth').val(data.DateOfBirth);

        loadEmployeeEducations(data.Id);
    });
}

function loadEmployeeAddressInfo(code) {
    $.get('/AdminEmployeeInformation/GetEmployeeAddressInfo', { employeeCode: code }, function (data) {
        if (!data) return;

        $('#PresentHouseVillageName').val(data.PresentHouseVillageName);
        $('#PresentHouseNo').val(data.PresentHouseNo);
        $('#PresentRoadNo').val(data.PresentRoadNo);
        $('#PresentBlock').val(data.PresentBlock);
        $('#PresentArea').val(data.PresentArea);
        $('#PresentSector').val(data.PresentSector);
        $('#PresentCountry').val(data.PresentCountry);
        $('#PresentDivision').val(data.PresentDivision);
        $('#PresentDistrict').val(data.PresentDistrict);
        $('#PresentThanaUpazilla').val(data.PresentThanaUpazilla);
        $('#PresentPostOffice').val(data.PresentPostOffice);

        $('#PermanentHouseVillageName').val(data.PermanentHouseVillageName);
        $('#PermanentHouseNo').val(data.PermanentHouseNo);
        $('#PermanentRoadNo').val(data.PermanentRoadNo);
        $('#PermanentBlock').val(data.PermanentBlock);
        $('#PermanentArea').val(data.PermanentArea);
        $('#PermanentSector').val(data.PermanentSector);
        $('#PermanentCountry').val(data.PermanentCountry);
        $('#PermanentDivision').val(data.PermanentDivision);
        $('#PermanentDistrict').val(data.PermanentDistrict);
        $('#PermanentThanaUpazilla').val(data.PermanentThanaUpazilla);
        $('#PermanentPostOffice').val(data.PermanentPostOffice);
    });
}

function saveGeneralInfo() {
    var dto = {
        Id: $('#EmployeeDbId').val(),
        EmployeeCode: $('#EmployeeCode').val(),
        EmployeeName: $('#EmployeeName').val(),
        FatherName: $('#FatherName').val(),
        MotherName: $('#MotherName').val(),
        MobileNo: $('#MobileNo').val(),
        BasicSalary: $('#BasicSalary').val(),
        DateOfBirth: $('#DateOfBirth').val()
    };

    $.ajax({
        url: '/AdminEmployeeInformation/UpdateEmployeeGeneralInfo',
        type: 'POST',
        data: dto,
        success: function () {
            alert('General information saved successfully.');
        },
        error: function () {
            alert('Save failed.');
        }
    });
}

function saveAddressInfo() {
    var dto = {
        PresentHouseVillageName: $('#PresentHouseVillageName').val(),
        PresentHouseNo: $('#PresentHouseNo').val(),
        PresentRoadNo: $('#PresentRoadNo').val(),
        PresentBlock: $('#PresentBlock').val(),
        PresentArea: $('#PresentArea').val(),
        PresentSector: $('#PresentSector').val(),
        PresentCountry: $('#PresentCountry').val(),
        PresentDivision: $('#PresentDivision').val(),
        PresentDistrict: $('#PresentDistrict').val(),
        PresentThanaUpazilla: $('#PresentThanaUpazilla').val(),
        PresentPostOffice: $('#PresentPostOffice').val(),

        PermanentHouseVillageName: $('#PermanentHouseVillageName').val(),
        PermanentHouseNo: $('#PermanentHouseNo').val(),
        PermanentRoadNo: $('#PermanentRoadNo').val(),
        PermanentBlock: $('#PermanentBlock').val(),
        PermanentArea: $('#PermanentArea').val(),
        PermanentSector: $('#PermanentSector').val(),
        PermanentCountry: $('#PermanentCountry').val(),
        PermanentDivision: $('#PermanentDivision').val(),
        PermanentDistrict: $('#PermanentDistrict').val(),
        PermanentThanaUpazilla: $('#PermanentThanaUpazilla').val(),
        PermanentPostOffice: $('#PermanentPostOffice').val()
    };

    $.ajax({
        url: '/AdminEmployeeInformation/UpdateEmployeeAddressInfo',
        type: 'POST',
        data: {
            employeeId: $('#EmployeeDbId').val(),
            dto: dto
        },
        success: function () {
            alert('Address information saved successfully.');
        },
        error: function () {
            alert('Address save failed.');
        }
    });
}

function loadEmployeeEducations(employeeId) {
    $.get('/AdminEmployeeInformation/GetEmployeeEducations', { employeeId: employeeId }, function (data) {
        var tbody = $('#educationTable tbody');
        tbody.empty();

        $.each(data || [], function (i, item) {
            var row = '<tr data-id="' + item.Id + '">' +
                '<td data-educationid="' + item.EducationId + '">' + (item.EducationName || '') + '</td>' +
                '<td data-group="' + htmlEncode(item.Group) + '">' + (item.Group || '') + '</td>' +
                '<td data-board="' + htmlEncode(item.Board) + '">' + (item.Board || '') + '</td>' +
                '<td data-year="' + htmlEncode(item.AcademicYear) + '">' + (item.AcademicYear || '') + '</td>' +
                '<td data-institute="' + htmlEncode(item.AcademicInstitute) + '">' + (item.AcademicInstitute || '') + '</td>' +
                '<td data-division="' + htmlEncode(item.Division) + '">' + (item.Division || '') + '</td>' +
                '<td data-result="' + htmlEncode(item.Result) + '">' + (item.Result || '') + '</td>' +
                '<td>' +
                '<button type="button" class="btn btn-sm btn-primary me-1" onclick="editEducationRow(this)">Edit</button> ' +
                '<button type="button" class="btn btn-sm btn-danger" onclick="deleteEducationRow(' + item.Id + ')">Delete</button>' +
                '</td>' +
                '</tr>';

            tbody.append(row);
        });
    });
}

function addEducationRow() {
    var row = '<tr data-id="0">' +
        '<td><select class="form-control edu-id">' + getEducationOptions('') + '</select></td>' +
        '<td><select class="form-control edu-group">' + getStringOptions(educationFieldDropdowns.Groups, '') + '</select></td>' +
        '<td><select class="form-control edu-board">' + getStringOptions(educationFieldDropdowns.Boards, '') + '</select></td>' +
        '<td><select class="form-control edu-year">' + getStringOptions(educationFieldDropdowns.AcademicYears, '') + '</select></td>' +
        '<td><select class="form-control edu-institute">' + getStringOptions(educationFieldDropdowns.AcademicInstitutes, '') + '</select></td>' +
        '<td><select class="form-control edu-division">' + getStringOptions(educationFieldDropdowns.Divisions, '') + '</select></td>' +
        '<td><select class="form-control edu-result">' + getStringOptions(educationFieldDropdowns.Results, '') + '</select></td>' +
        '<td><button type="button" class="btn btn-sm btn-success" onclick="saveEducationRow(this)">Save</button></td>' +
        '</tr>';

    $('#educationTable tbody').append(row);
}

function editEducationRow(btn) {
    var row = $(btn).closest('tr');
    var id = row.attr('data-id');
    var tds = row.find('td');

    var educationId = $(tds[0]).attr('data-educationid');
    var group = $(tds[1]).text().trim();
    var board = $(tds[2]).text().trim();
    var year = $(tds[3]).text().trim();
    var institute = $(tds[4]).text().trim();
    var division = $(tds[5]).text().trim();
    var result = $(tds[6]).text().trim();

    row.html(
        '<td><select class="form-control edu-id">' + getEducationOptions(educationId) + '</select></td>' +
        '<td><select class="form-control edu-group">' + getStringOptions(educationFieldDropdowns.Groups, group) + '</select></td>' +
        '<td><select class="form-control edu-board">' + getStringOptions(educationFieldDropdowns.Boards, board) + '</select></td>' +
        '<td><select class="form-control edu-year">' + getStringOptions(educationFieldDropdowns.AcademicYears, year) + '</select></td>' +
        '<td><select class="form-control edu-institute">' + getStringOptions(educationFieldDropdowns.AcademicInstitutes, institute) + '</select></td>' +
        '<td><select class="form-control edu-division">' + getStringOptions(educationFieldDropdowns.Divisions, division) + '</select></td>' +
        '<td><select class="form-control edu-result">' + getStringOptions(educationFieldDropdowns.Results, result) + '</select></td>' +
        '<td><button type="button" class="btn btn-sm btn-success" onclick="saveEducationRow(this)">Save</button></td>'
    );

    row.attr('data-id', id);
}

function saveEducationRow(btn) {
    var row = $(btn).closest('tr');

    var dto = {
        Id: row.attr('data-id'),
        EmployeeId: $('#EmployeeDbId').val(),
        EducationId: row.find('.edu-id').val(),
        Group: row.find('.edu-group').val(),
        Board: row.find('.edu-board').val(),
        AcademicYear: row.find('.edu-year').val(),
        AcademicInstitute: row.find('.edu-institute').val(),
        Division: row.find('.edu-division').val(),
        Result: row.find('.edu-result').val()
    };

    $.ajax({
        url: '/AdminEmployeeInformation/SaveEducation',
        type: 'POST',
        data: dto,
        success: function () {
            loadEmployeeEducations($('#EmployeeDbId').val());
        },
        error: function () {
            alert('Education save failed.');
        }
    });
}

function deleteEducationRow(id) {
    if (!confirm('Are you sure you want to delete this row?')) {
        return;
    }

    $.ajax({
        url: '/AdminEmployeeInformation/DeleteEducation',
        type: 'POST',
        data: { id: id },
        success: function () {
            loadEmployeeEducations($('#EmployeeDbId').val());
        },
        error: function () {
            alert('Delete failed.');
        }
    });
}

function getEducationOptions(selectedId) {
    var html = '<option value="">-- Select --</option>';

    $.each(educationDropdown, function (i, item) {
        var selected = String(item.Id) === String(selectedId) ? ' selected="selected"' : '';
        html += '<option value="' + item.Id + '"' + selected + '>' + item.EducationName + '</option>';
    });

    return html;
}

function getStringOptions(list, selectedValue) {
    var html = '<option value="">-- Select --</option>';

    $.each(list || [], function (i, item) {
        var selected = String(item) === String(selectedValue) ? ' selected="selected"' : '';
        html += '<option value="' + htmlEncode(item) + '"' + selected + '>' + htmlEncode(item) + '</option>';
    });

    return html;
}

function htmlEncode(value) {
    return $('<div/>').text(value || '').html();
}