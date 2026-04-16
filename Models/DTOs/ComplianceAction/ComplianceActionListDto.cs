namespace AttendanceSyncApp.Models.DTOs.ComplianceAction
{
    public class ComplianceActionListDto
    {
        public int Id { get; set; }
        public int? EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string OffenceType { get; set; }
        public string OffenceDetails { get; set; }
        public string ComplianceActionType { get; set; }
        public string ComplianceActionDetails { get; set; }
    }
}