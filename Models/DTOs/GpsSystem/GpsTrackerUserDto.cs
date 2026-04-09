namespace AttendanceSyncApp.Models.DTOs.GpsSystem
{
    public class GpsTrackerUserDto
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string MobileNo { get; set; }
        public string DepartmentName { get; set; }
        public string BranchName { get; set; }
        public bool IsActive { get; set; }
        public bool IsLiveTrackingEnabled { get; set; }
        public bool IsFieldVisitEnabled { get; set; }
        public string CreatedAtText { get; set; }
    }
}