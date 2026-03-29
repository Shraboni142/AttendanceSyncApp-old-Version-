namespace AttendanceSyncApp.Models.DTOs.Reports
{
    public class EmployeeJobCardDayDetailDto
    {
        public string CompanyName { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string Branch { get; set; }
        public string Section { get; set; }
        public string Location { get; set; }
        public string DOJ { get; set; }

        public string AttendanceDate { get; set; }
        public string DayStatus { get; set; }

        public string LoginTime { get; set; }
        public string LogoutTime { get; set; }
        public string ExpectedLoginTime { get; set; }
        public string ExpectedLogoutTime { get; set; }
        public string LoginWorkstationId { get; set; }
        public string LogoutWorkstationId { get; set; }
        public string Processcode { get; set; }
        public string BranchId { get; set; }
        public string IsManualLogin { get; set; }
        public string IsLate { get; set; }
        public string AbsentCountingTime { get; set; }
        public string WorkingHours { get; set; }
        public string Remarks { get; set; }
    }
}