namespace AttendanceSyncApp.Models.DTOs.Reports
{
    public class EmployeeJobCardAllReportRowDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public int Present { get; set; }
        public int Absent { get; set; }
    }
}