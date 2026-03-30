using System.Collections.Generic;

namespace AttendanceSyncApp.Models.DTOs.Reports
{
    public class EmployeeJobCardAllReportViewDto
    {
        public string ReportTitle { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string FromDateText { get; set; }
        public string ToDateText { get; set; }
        public List<EmployeeJobCardAllReportRowDto> Rows { get; set; }

        public EmployeeJobCardAllReportViewDto()
        {
            Rows = new List<EmployeeJobCardAllReportRowDto>();
        }
    }
}