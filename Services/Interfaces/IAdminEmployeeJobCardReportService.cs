using System.Collections.Generic;
using AttendanceSyncApp.Models.DTOs.Reports;

namespace AttendanceSyncApp.Services.Interfaces
{
    public interface IAdminEmployeeJobCardReportService
    {
        List<EmployeeDropdownDto> GetEmployees(int serverId, string databaseName);
        EmployeeJobCardHeaderDto GetHeaderData(int serverId, string databaseName, int employeeId, string fromDateText, string toDateText);
        List<EmployeeJobCardDetailDto> GetDetailData(int serverId, string databaseName, int employeeId, string fromDate, string toDate);
        EmployeeJobCardSummaryDto GetSummaryData(List<EmployeeJobCardDetailDto> details);
    }
}