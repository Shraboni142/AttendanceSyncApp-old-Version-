using System;

namespace AttendanceSyncApp.Models.DTOs.Reports
{
    public class EmployeeJobCardHeaderDto
    {
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }

        public string ReportTitle { get; set; }
        public string FromDateText { get; set; }
        public string ToDateText { get; set; }

        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string DOJ { get; set; }
        public string Section { get; set; }
        public string Branch { get; set; }
        public string Location { get; set; }
    }
}