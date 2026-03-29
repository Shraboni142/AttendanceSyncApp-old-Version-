namespace AttendanceSyncApp.Models.DTOs.Reports
{
    public class EmployeeJobCardDayDetailFilterDto
    {
        public int ServerId { get; set; }
        public string DatabaseName { get; set; }
        public int EmployeeId { get; set; }
        public string SelectedDate { get; set; }
    }
}