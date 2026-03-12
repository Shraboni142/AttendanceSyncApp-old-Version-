using System.Collections.Generic;
using AttendanceSyncApp.Models.DTOs.Reports;

namespace AttendanceSyncApp.Services.Interfaces
{
    public interface IAdminEmployeeStatusReportService
    {
        List<EmployeeStatusReportRowDto> GetEmployeeStatusReport(
            int serverId,
            List<string> databases,
            int status);
    }
}