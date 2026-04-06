namespace AttendanceSyncApp.Models.DTOs.EmployeeInformation
{
    public class EmployeeProfileListDto
    {
        public int GeneralInfoId { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string HomeAddress { get; set; }
    }
}