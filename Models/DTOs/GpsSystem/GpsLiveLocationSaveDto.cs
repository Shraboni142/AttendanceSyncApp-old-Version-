namespace AttendanceSyncApp.Models.DTOs.GpsSystem
{
    public class GpsLiveLocationSaveDto
    {
        public int TrackerUserId { get; set; }
        public string MobileNo { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public decimal? AccuracyMeter { get; set; }
        public string DeviceInfo { get; set; }
        public string IpAddress { get; set; }
    }

}