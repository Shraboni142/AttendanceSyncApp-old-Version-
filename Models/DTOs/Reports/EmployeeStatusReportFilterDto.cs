using System.Collections.Generic;

namespace AttendanceSyncApp.Models.DTOs.Reports
{
    public class EmployeeStatusReportFilterDto
    {
        public int ServerIpId { get; set; }

        public List<string> SelectedDatabases { get; set; }

        public int Status { get; set; }
    }
}