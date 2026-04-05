alert('NEW JS LOADED');
let educationDropdown = [];
let educationFieldDropdowns = {
    Groups: [],
    Boards: [],
    AcademicYears: [],
    AcademicInstitutes: [],
    Divisions: [],
    Results: []
};

let employeeList = [];
let educationRows = [];

$(document).ready(function () {
    loadEmployees();
    loadEducationDropdown();
    loadEducationFieldDropdowns();
    loadDesignations();
    loadDepartments();

    $('#EmployeeCode').on('change', function () {
        var code = ($(this).val() || '').trim();
        if (!code) return;

        loadEmployeeGeneralInfo(code);
        loadEmployeeAddressInfo(code);
    });

    $('#EmployeeName').on('change', function () {
        var inputName = ($(this).val() || '').trim();
        if (!inputName) return;

        var matchedEmployee = employeeList.find(function (x) {
            return (x.EmployeeName || '').trim().toLowerCase() === inputName.toLowerCase();
        });

        if (!matchedEmployee) return;

        $('#EmployeeCode').val(matchedEmployee.EmployeeCode);
        loadEmployeeGeneralInfo(matchedEmployee.EmployeeCode);
        loadEmployeeAddressInfo(matchedEmployee.EmployeeCode);
    });

    $('#btnAddEducationRow').on('click', function () {
        addEducationRow();
    });

    $('#btnSaveAllEmployeeInformation').on('click', function () {
        saveAllEmployeeInformation();
    });

    $('#SameAsPresent').on('change', function () {
        if ($(this).is(':checked')) {
            copyPresentToPermanent();
        }
    });
});

function loadEmployees() {
    $.get('/AdminEmployeeInformation/GetEmployees', function (data) {
        employeeList = data || [];

        var codeList = $('#employeeCodeList');
        var nameList = $('#employeeNameList');

        if (codeList.length) codeList.empty();
        if (nameList.length) nameList.empty();

        $.each(employeeList, function (i, item) {
            if (codeList.length) {
                codeList.append('<option value="' + htmlEncode(item.EmployeeCode) + '"></option>');
            }
            if (nameList.length) {
                nameList.append('<option value="' + htmlEncode(item.EmployeeName) + '"></option>');
            }
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

        $.each(data || [], function (i, item) {
            $dropdown.append('<option value="' + item.Id + '">' + htmlEncode(item.Name) + '</option>');
        });
    });
}

function loadDepartments() {
    $.get('/AdminEmployeeInformation/GetDepartments', function (data) {
        var $dropdown = $('#DepartmentName');
        $dropdown.empty();
        $dropdown.append('<option value="">-- Select Department --</option>');

        $.each(data || [], function (i, item) {
            $dropdown.append('<option value="' + item.Id + '">' + htmlEncode(item.Name) + '</option>');
        });
    });
}

function loadEmployeeGeneralInfo(code) {
    $.get('/AdminEmployeeInformation/GetEmployeeGeneralInfo', { employeeCode: code }, function (data) {
        if (!data) return;

        $('#EmployeeDbId').val(data.Id || 0);
        $('#GeneralInfoId').val(data.GeneralInfoId || 0);

        $('#EmployeeCode').val(data.EmployeeCode || '');
        $('#EmployeeName').val(data.EmployeeName || '');
        $('#FatherName').val(data.FatherName || '');
        $('#MotherName').val(data.MotherName || '');
        $('#MobileNo').val(data.MobileNo || '');
        $('#BranchName').val(data.BranchName || '');
        $('#BasicSalary').val(data.BasicSalary || '');
        $('#DateOfBirth').val(data.DateOfBirth || '');

        $('#DesignationName').val(data.DesignationId || '');
        $('#DepartmentName').val(data.DepartmentId || '');

        loadEmployeeEducations(data.Id);
    });
}

function loadEmployeeAddressInfo(code) {
    $.get('/AdminEmployeeInformation/GetEmployeeAddressInfo', { employeeCode: code }, function (data) {
        if (!data) return;

        $('#PresentHouseVillageName').val(data.PresentHouseVillageName || '');
        $('#PresentHouseNo').val(data.PresentHouseNo || '');
        $('#PresentRoadNo').val(data.PresentRoadNo || '');
        $('#PresentBlock').val(data.PresentBlock || '');
        $('#PresentArea').val(data.PresentArea || '');
        $('#PresentSector').val(data.PresentSector || '');
        $('#PresentCountry').val(data.PresentCountry || '');
        $('#PresentDivision').val(data.PresentDivision || '');
        $('#PresentDistrict').val(data.PresentDistrict || '');
        $('#PresentThanaUpazilla').val(data.PresentThanaUpazilla || '');
        $('#PresentPostOffice').val(data.PresentPostOffice || '');

        $('#PermanentHouseVillageName').val(data.PermanentHouseVillageName || '');
        $('#PermanentHouseNo').val(data.PermanentHouseNo || '');
        $('#PermanentRoadNo').val(data.PermanentRoadNo || '');
        $('#PermanentBlock').val(data.PermanentBlock || '');
        $('#PermanentArea').val(data.PermanentArea || '');
        $('#PermanentSector').val(data.PermanentSector || '');
        $('#PermanentCountry').val(data.PermanentCountry || '');
        $('#PermanentDivision').val(data.PermanentDivision || '');
        $('#PermanentDistrict').val(data.PermanentDistrict || '');
        $('#PermanentThanaUpazilla').val(data.PermanentThanaUpazilla || '');
        $('#PermanentPostOffice').val(data.PermanentPostOffice || '');
    });
}

function loadEmployeeEducations(employeeId) {
    $.get('/AdminEmployeeInformation/GetEmployeeEducations', { employeeId: employeeId }, function (data) {
        educationRows = $.map(data || [], function (item) {
            return {
                Id: item.Id || 0,
                EmployeeId: item.EmployeeId || 0,
                EducationId: item.EducationId || 0,
                EducationName: item.EducationName || '',
                Group: item.Group || '',
                Board: item.Board || '',
                AcademicYear: item.AcademicYear || '',
                AcademicInstitute: item.AcademicInstitute || '',
                Division: item.Division || '',
                Result: item.Result || '',
                IsEditMode: false
            };
        });

        renderEducationRows();
    });
}

function addEducationRow() {
    educationRows.push({
        Id: 0,
        EmployeeId: parseInt($('#EmployeeDbId').val() || '0'),
        EducationId: 0,
        EducationName: '',
        Group: '',
        Board: '',
        AcademicYear: '',
        AcademicInstitute: '',
        Division: '',
        Result: '',
        IsEditMode: true
    });

    renderEducationRows();
}

function editEducationRow(index) {
    if (educationRows[index]) {
        educationRows[index].IsEditMode = true;
        renderEducationRows();
    }
}

function saveEducationRow(index) {
    var $row = $('#educationTable tbody tr[data-index="' + index + '"]');
    if (!$row.length) return;

    var educationId = parseInt($row.find('.edu-id').val() || '0');
    var educationName = educationId > 0 ? $row.find('.edu-id option:selected').text() : '';

    educationRows[index] = {
        Id: 0,
        EmployeeId: 0,
        EducationId: educationId,
        EducationName: educationName,
        Group: $row.find('.edu-group').val() || '',
        Board: $row.find('.edu-board').val() || '',
        AcademicYear: $row.find('.edu-year').val() || '',
        AcademicInstitute: $row.find('.edu-institute').val() || '',
        Division: $row.find('.edu-division').val() || '',
        Result: $row.find('.edu-result').val() || '',
        IsEditMode: false
    };

    renderEducationRows();
}
function deleteEducationRow(index) {
    if (!confirm('Are you sure you want to delete this row?')) {
        return;
    }

    educationRows.splice(index, 1);
    renderEducationRows();
}

function renderEducationRows() {
    var tbody = $('#educationTable tbody');
    tbody.empty();

    $.each(educationRows, function (index, item) {
        if (item.IsEditMode) {
            var editRow = '<tr data-index="' + index + '" data-id="' + (item.Id || 0) + '">' +
                '<td><select class="form-control edu-id">' + getEducationOptions(item.EducationId) + '</select></td>' +
                '<td><select class="form-control edu-group">' + getStringOptions(educationFieldDropdowns.Groups, item.Group) + '</select></td>' +
                '<td><select class="form-control edu-board">' + getStringOptions(educationFieldDropdowns.Boards, item.Board) + '</select></td>' +
                '<td><select class="form-control edu-year">' + getStringOptions(educationFieldDropdowns.AcademicYears, item.AcademicYear) + '</select></td>' +
                '<td><select class="form-control edu-institute">' + getStringOptions(educationFieldDropdowns.AcademicInstitutes, item.AcademicInstitute) + '</select></td>' +
                '<td><select class="form-control edu-division">' + getStringOptions(educationFieldDropdowns.Divisions, item.Division) + '</select></td>' +
                '<td><select class="form-control edu-result">' + getStringOptions(educationFieldDropdowns.Results, item.Result) + '</select></td>' +
                '<td>' +
                '<button type="button" class="btn btn-sm btn-success me-1" onclick="saveEducationRow(' + index + ')">LOCAL ONLY</button>'
                '<button type="button" class="btn btn-sm btn-danger" onclick="deleteEducationRow(' + index + ')">Delete</button>' +
                '</td>' +
                '</tr>';

            tbody.append(editRow);
        } else {
            var displayRow = '<tr data-index="' + index + '" data-id="' + (item.Id || 0) + '">' +
                '<td>' + htmlEncode(item.EducationName) + '</td>' +
                '<td>' + htmlEncode(item.Group) + '</td>' +
                '<td>' + htmlEncode(item.Board) + '</td>' +
                '<td>' + htmlEncode(item.AcademicYear) + '</td>' +
                '<td>' + htmlEncode(item.AcademicInstitute) + '</td>' +
                '<td>' + htmlEncode(item.Division) + '</td>' +
                '<td>' + htmlEncode(item.Result) + '</td>' +
                '<td>' +
                '<button type="button" class="btn btn-sm btn-primary me-1" onclick="editEducationRow(' + index + ')">Edit</button>' +
                '<button type="button" class="btn btn-sm btn-danger" onclick="deleteEducationRow(' + index + ')">Delete</button>' +
                '</td>' +
                '</tr>';

            tbody.append(displayRow);
        }
    });
}

function getEducationRows() {
    var rows = [];

    $.each(educationRows, function (index, sourceRow) {
        rows.push({
            Id: sourceRow.Id || 0,
            EmployeeId: parseInt($('#EmployeeDbId').val() || '0'),
            EducationId: sourceRow.EducationId || 0,
            EducationName: sourceRow.EducationName || '',
            Group: sourceRow.Group || '',
            Board: sourceRow.Board || '',
            AcademicYear: sourceRow.AcademicYear || '',
            AcademicInstitute: sourceRow.AcademicInstitute || '',
            Division: sourceRow.Division || '',
            Result: sourceRow.Result || ''
        });
    });

    return rows;
}

function saveAllEmployeeInformation() {
    var employeeName = ($('#EmployeeName').val() || '').trim();
    var employeeCode = ($('#EmployeeCode').val() || '').trim();

    if (!employeeName || !employeeCode) {
        alert('Please enter Employee Name and Employee Code first.');
        return;
    }

    if ($('#SameAsPresent').is(':checked')) {
        copyPresentToPermanent();
    }

    var scrollPos = $(window).scrollTop();

    var payload = {
        GeneralInfo: {
            GeneralInfoId: parseInt($('#GeneralInfoId').val() || '0'),
            Id: parseInt($('#EmployeeDbId').val() || '0'),
            EmployeeCode: employeeCode,
            EmployeeName: employeeName,
            FatherName: $('#FatherName').val(),
            MotherName: $('#MotherName').val(),
            MobileNo: $('#MobileNo').val(),
            BasicSalary: $('#BasicSalary').val() ? parseFloat($('#BasicSalary').val()) : null,
            DateOfBirth: $('#DateOfBirth').val(),
            DesignationId: $('#DesignationName').val() ? parseInt($('#DesignationName').val()) : null,
            DepartmentId: $('#DepartmentName').val() ? parseInt($('#DepartmentName').val()) : null,
            DesignationName: $('#DesignationName option:selected').val() ? $('#DesignationName option:selected').text() : '',
            DepartmentName: $('#DepartmentName option:selected').val() ? $('#DepartmentName option:selected').text() : '',
            BranchName: $('#BranchName').val()
        },
        AddressInfo: {
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
        },
        Educations: getEducationRows()
    };

    $.ajax({
        url: '/AdminEmployeeInformation/SaveAllEmployeeInformation',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(payload),
        success: function (response) {
            $(window).scrollTop(scrollPos);

            if (response && response.success) {
                alert(response.message || 'All employee information saved successfully.');

                loadEmployeeGeneralInfo(employeeCode);
                loadEmployeeAddressInfo(employeeCode);

                setTimeout(function () {
                    $(window).scrollTop(scrollPos);
                }, 100);
            } else {
                alert(response && response.message ? response.message : 'Save failed.');
            }
        },
        error: function (xhr) {
            $(window).scrollTop(scrollPos);
            alert(xhr.responseText || 'Save failed.');
        }
    });
}

function copyPresentToPermanent() {
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

function getEducationOptions(selectedId) {
    var html = '<option value="">-- Select --</option>';

    $.each(educationDropdown, function (i, item) {
        var selected = String(item.Id) === String(selectedId) ? ' selected="selected"' : '';
        html += '<option value="' + item.Id + '"' + selected + '>' + htmlEncode(item.EducationName) + '</option>';
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