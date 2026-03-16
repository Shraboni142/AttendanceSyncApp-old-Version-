namespace AttendanceSyncApp.Models.DTOs.Reports
{
    public class EmployeeStatusReportRowDto
    {
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string DesignationName { get; set; }
        public decimal? BasicSalary { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public bool IsActive { get; set; }
    }
}