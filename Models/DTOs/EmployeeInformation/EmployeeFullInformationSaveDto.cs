using System.Collections.Generic;

namespace AttendanceSyncApp.Models.DTOs.EmployeeInformation
{
    public class EmployeeFullInformationSaveDto
    {
        public EmployeeInfoGeneralDto GeneralInfo { get; set; }
        public EmployeeInfoAddressDto AddressInfo { get; set; }
        public List<EmployeeEducationDto> Educations { get; set; }
        public int GeneralInfoId { get; set; }
        public int? DesignationId { get; set; }
        public int? DepartmentId { get; set; }
    }
}