namespace AttendanceSyncApp.Models.DTOs.Reports
{
    public class EmployeeJobCardSummaryDto
    {
        public int TotalDays { get; set; }
        public int TotalWeekendDays { get; set; }
        public int TotalPresentDays { get; set; }
        public int TotalWorkingDays { get; set; }
        public int TotalAbsentDays { get; set; }
        public int TotalLateDays { get; set; }
        public int TotalHolidays { get; set; }
        public int TotalLeaveDays { get; set; }
        public int TotalAttendance { get; set; }
    }
}