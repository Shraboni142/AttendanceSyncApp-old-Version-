using System.Collections.Generic;
using AttendanceSyncApp.Models.DTOs.EmployeeInformation;

namespace AttendanceSyncApp.Services.Interfaces
{
    public interface IAdminEmployeeInformationService
    {
        List<EmployeeInfoDropdownDto> GetEmployees();
        EmployeeInfoGeneralDto GetEmployeeGeneralInfo(string employeeCode);
        bool UpdateEmployeeGeneralInfo(EmployeeInfoGeneralDto dto);

        EmployeeInfoAddressDto GetEmployeeAddressInfo(string employeeCode);
        bool UpdateEmployeeAddressInfo(string employeeCode, EmployeeInfoAddressDto dto);

        List<EmployeeEducationDto> GetEmployeeEducations(int employeeId);
        List<EducationDropdownDto> GetEducationDropdown();
        EducationFieldDropdownsDto GetEducationFieldDropdowns();
        bool SaveEducation(string employeeCode, EmployeeEducationDto dto);
        bool SaveAllEmployeeInformation(EmployeeFullInformationSaveDto dto);
        bool DeleteEducation(int id);
        List<DropdownItemDto> GetDesignations();
        List<DropdownItemDto> GetDepartments();
    }
}