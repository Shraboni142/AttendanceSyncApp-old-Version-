namespace AttendanceSyncApp.Models.DTOs.Reports
{
    public class EmployeeDropdownDto
    {
        public int Id { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string DisplayText { get; set; }
    }
}