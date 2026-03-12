using System;

namespace AttendanceSyncApp.Models.DTOs.Reports
{
    public class EmployeeJobCardReportFilterDto
    {
        public int ServerId { get; set; }
        public string DatabaseName { get; set; }
        public int EmployeeId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}