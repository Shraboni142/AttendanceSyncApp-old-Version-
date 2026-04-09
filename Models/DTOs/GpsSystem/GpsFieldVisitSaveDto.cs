namespace AttendanceSyncApp.Models.DTOs.GpsSystem
{
    public class GpsFieldVisitSaveDto
    {
        public int TrackerUserId { get; set; }
        public int? UserId { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string MobileNo { get; set; }
        public string VisitPurpose { get; set; }
        public string ClientName { get; set; }
        public string CollectionNo { get; set; }
        public string Remarks { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public decimal? AccuracyMeter { get; set; }
        public string AddressText { get; set; }
    }
}