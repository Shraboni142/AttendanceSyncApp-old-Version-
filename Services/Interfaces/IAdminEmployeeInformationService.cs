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
        bool UpdateEmployeeAddressInfo(int employeeId, EmployeeInfoAddressDto dto);

        List<EmployeeEducationDto> GetEmployeeEducations(int employeeId);
        List<EducationDropdownDto> GetEducationDropdown();
        EducationFieldDropdownsDto GetEducationFieldDropdowns();
        bool SaveEducation(EmployeeEducationDto dto);
        bool DeleteEducation(int id);
    }
}