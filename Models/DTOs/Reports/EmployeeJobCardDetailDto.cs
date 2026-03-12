namespace AttendanceSyncApp.Models.DTOs.Reports
{
    public class EmployeeJobCardDetailDto
    {
        public string Date { get; set; }
        public string Shift { get; set; }
        public string InTime { get; set; }
        public string OutTime { get; set; }
        public string LateMinutes { get; set; }
        public string OTHours { get; set; }
        public string DayStatus { get; set; }
        public string Remarks { get; set; }
    }
}