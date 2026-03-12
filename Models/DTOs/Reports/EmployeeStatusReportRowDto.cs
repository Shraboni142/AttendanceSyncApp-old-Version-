namespace AttendanceSyncApp.Models.DTOs.Reports
{
    public class EmployeeStatusReportRowDto
    {
        public string DatabaseName { get; set; }

        public int Id { get; set; }

        public int? BranchId { get; set; }

        public int? GradeScaleId { get; set; }

        public string MobileNo { get; set; }

        public int? DesignationId { get; set; }

        public decimal? BasicSalary { get; set; }
    }
}