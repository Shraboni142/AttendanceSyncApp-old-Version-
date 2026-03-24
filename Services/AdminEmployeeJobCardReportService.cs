using AttandanceSyncApp.Helpers;
using AttandanceSyncApp.Repositories;
using AttendanceSyncApp.Models.DTOs.Reports;
using AttendanceSyncApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;

using System.Linq;


namespace AttendanceSyncApp.Services
{
    public class AdminEmployeeJobCardReportService : IAdminEmployeeJobCardReportService
    {
        public List<EmployeeDropdownDto> GetEmployees(int serverId, string databaseName)
        {
            var result = new List<EmployeeDropdownDto>();

            string connString = GetConnectionString(serverId, databaseName);

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                string query = @"
                    SELECT
                        Id,
                        EmployeeId,
                        LTRIM(RTRIM(
                            ISNULL(FirstName, '') + ' ' +
                            ISNULL(MiddleName, '') + ' ' +
                            ISNULL(LastName, '')
                        )) AS EmployeeName
                    FROM dbo.Employees
                    ORDER BY EmployeeId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var id = Convert.ToInt32(reader["Id"]);
                        var employeeCode = reader["EmployeeId"] == DBNull.Value ? "" : reader["EmployeeId"].ToString();
                        var employeeName = reader["EmployeeName"] == DBNull.Value ? "" : reader["EmployeeName"].ToString();

                        result.Add(new EmployeeDropdownDto
                        {
                            Id = id,
                            EmployeeCode = employeeCode,
                            EmployeeName = employeeName,
                            DisplayText = employeeCode + " - " + employeeName
                        });
                    }
                }
            }

            return result;
        }

        public EmployeeJobCardHeaderDto GetHeaderData(int serverId, string databaseName, int employeeId, string fromDateText, string toDateText)
        {
            var result = new EmployeeJobCardHeaderDto
            {
                ReportTitle = "Employee Job Card",
                FromDateText = fromDateText,
                ToDateText = toDateText
            };

            string connString = GetConnectionString(serverId, databaseName);

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                string query = @"
                    SELECT TOP 1
                        c.CompanyName,
                        c.Address,
                        c.Phone,
                        c.Fax,
                        c.Email,

                        e.EmployeeId AS EmployeeCode,
                        LTRIM(RTRIM(
                            ISNULL(e.FirstName, '') + ' ' +
                            ISNULL(e.MiddleName, '') + ' ' +
                            ISNULL(e.LastName, '')
                        )) AS EmployeeName,
                        d.DesignationName,
                        dp.DepartmentName,
                        e.JoiningDate,
                        s.SectionName,
                        b.BranchName,
                        l.LocationName
                    FROM dbo.Employees e
                    CROSS JOIN dbo.Companies c
                    LEFT JOIN dbo.Designations d ON e.DesignationId = d.Id
                    LEFT JOIN dbo.Departments dp ON e.DepartmentId = dp.Id
                    LEFT JOIN dbo.Sections s ON e.SectionId = s.Id
                    LEFT JOIN dbo.Branches b ON e.BranchId = b.Id
                    LEFT JOIN dbo.Locations l ON e.LocationId = l.Id
                    WHERE e.Id = @EmployeeId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeId", employeeId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result.CompanyName = reader["CompanyName"] == DBNull.Value ? "" : reader["CompanyName"].ToString();
                            result.Address = reader["Address"] == DBNull.Value ? "" : reader["Address"].ToString();
                            result.Phone = reader["Phone"] == DBNull.Value ? "" : reader["Phone"].ToString();
                            result.Fax = reader["Fax"] == DBNull.Value ? "" : reader["Fax"].ToString();
                            result.Email = reader["Email"] == DBNull.Value ? "" : reader["Email"].ToString();

                            result.EmployeeCode = reader["EmployeeCode"] == DBNull.Value ? "" : reader["EmployeeCode"].ToString();
                            result.EmployeeName = reader["EmployeeName"] == DBNull.Value ? "" : reader["EmployeeName"].ToString();
                            result.Designation = reader["DesignationName"] == DBNull.Value ? "" : reader["DesignationName"].ToString();
                            result.Department = reader["DepartmentName"] == DBNull.Value ? "" : reader["DepartmentName"].ToString();
                            result.Section = reader["SectionName"] == DBNull.Value ? "" : reader["SectionName"].ToString();
                            result.Branch = reader["BranchName"] == DBNull.Value ? "" : reader["BranchName"].ToString();
                            result.Location = reader["LocationName"] == DBNull.Value ? "" : reader["LocationName"].ToString();

                            result.DOJ = reader["JoiningDate"] == DBNull.Value
                                ? ""
                                : Convert.ToDateTime(reader["JoiningDate"]).ToString("dd-MM-yyyy");
                        }
                    }
                }
            }

            return result;
        }

        public List<string> GetDatabasesForServer(int serverId)
        {
            var databases = new List<string>();

            using (var unitOfWork = new AuthUnitOfWork())
            {
                var server = unitOfWork.ServerIps.GetById(serverId);
                if (server == null || !server.IsActive)
                {
                    return databases;
                }

                var accessibleDatabases = unitOfWork.DatabaseAccess
                    .GetAccessibleDatabasesByServerId(serverId)
                    .Select(x => x.DatabaseName)
                    .Distinct()
                    .ToList();

                if (!accessibleDatabases.Any())
                {
                    return databases;
                }

                var decryptedPassword = EncryptionHelper.Decrypt(server.DatabasePassword);

                string connectionString =
                    $"Server={server.IpAddress};Database=master;User Id={server.DatabaseUser};Password={decryptedPassword};TrustServerCertificate=True;";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                SELECT name
                FROM sys.databases
                WHERE database_id > 4
                ORDER BY name";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var dbName = reader["name"].ToString();

                            if (accessibleDatabases.Contains(dbName))
                            {
                                databases.Add(dbName);
                            }
                        }
                    }
                }
            }

            return databases;
        }
        public List<EmployeeJobCardDetailDto> GetDetailData(int serverId, string databaseName, int employeeId, string fromDate, string toDate)
        {
            var result = new List<EmployeeJobCardDetailDto>();

            string serverIp = GetServerIp(serverId);
            string username = "sa";
            string password = "open";

            string connString =
                $"Server={serverIp};Database={databaseName};User Id={username};Password={password};TrustServerCertificate=True;";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                string query = @"
;WITH DateRange AS
(
    SELECT CAST(@FromDate AS DATE) AS AttendanceDate
    UNION ALL
    SELECT DATEADD(DAY, 1, AttendanceDate)
    FROM DateRange
    WHERE AttendanceDate < CAST(@ToDate AS DATE)
),
LoginSummary AS
(
    SELECT
        CAST([Date] AS DATE) AS AttendanceDate,
        MIN(LoginTime) AS InTime,
        MAX(LogoutTime) AS OutTime
    FROM dbo.EmployeeManualLogins
    WHERE EmployeeId = @EmployeeId
      AND CAST([Date] AS DATE) BETWEEN CAST(@FromDate AS DATE) AND CAST(@ToDate AS DATE)
      AND LoginTime IS NOT NULL
    GROUP BY CAST([Date] AS DATE)
)
SELECT
    DR.AttendanceDate AS [Date],
    '' AS Shift,
    LS.InTime,
    LS.OutTime,
    '' AS LateMinutes,
    '' AS OTHours,
    CASE
        WHEN LS.InTime IS NOT NULL THEN 'Present'
        ELSE 'Absent'
    END AS DayStatus,
    '' AS Remarks
FROM DateRange DR
LEFT JOIN LoginSummary LS
    ON DR.AttendanceDate = LS.AttendanceDate
ORDER BY DR.AttendanceDate
OPTION (MAXRECURSION 1000);";

                using (SqlCommand attendanceCmd = new SqlCommand(query, conn))
                {
                    attendanceCmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                    attendanceCmd.Parameters.AddWithValue("@FromDate", Convert.ToDateTime(fromDate).Date);
                    attendanceCmd.Parameters.AddWithValue("@ToDate", Convert.ToDateTime(toDate).Date);

                    using (SqlDataReader reader = attendanceCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new EmployeeJobCardDetailDto
                            {
                                Date = Convert.ToDateTime(reader["Date"]).ToString("dd-MMM-yyyy"),
                                Shift = reader["Shift"] == DBNull.Value ? "" : reader["Shift"].ToString(),
                                InTime = reader["InTime"] == DBNull.Value
                                    ? ""
                                    : Convert.ToDateTime(reader["InTime"]).ToString("hh:mm tt"),
                                OutTime = reader["OutTime"] == DBNull.Value
                                    ? ""
                                    : Convert.ToDateTime(reader["OutTime"]).ToString("hh:mm tt"),
                                LateMinutes = reader["LateMinutes"] == DBNull.Value ? "" : reader["LateMinutes"].ToString(),
                                OTHours = reader["OTHours"] == DBNull.Value ? "" : reader["OTHours"].ToString(),
                                DayStatus = reader["DayStatus"] == DBNull.Value ? "" : reader["DayStatus"].ToString(),
                                Remarks = reader["Remarks"] == DBNull.Value ? "" : reader["Remarks"].ToString()
                            });
                        }
                    }
                }
            }

            return result;
        }

        public EmployeeJobCardSummaryDto GetSummaryData(List<EmployeeJobCardDetailDto> details)
        {
            var result = new EmployeeJobCardSummaryDto();

            if (details == null)
            {
                return result;
            }

            int totalDays = details.Count;
            int presentDays = 0;
            int absentDays = 0;

            foreach (var item in details)
            {
                if (string.Equals(item.DayStatus, "Present", StringComparison.OrdinalIgnoreCase))
                {
                    presentDays++;
                }
                else
                {
                    absentDays++;
                }
            }

            result.TotalDays = totalDays;
            result.TotalWeekendDays = 0;
            result.TotalPresentDays = presentDays;
            result.TotalWorkingDays = totalDays;
            result.TotalAbsentDays = absentDays;
            result.TotalLateDays = 0;
            result.TotalHolidays = 0;
            result.TotalLeaveDays = 0;
            result.TotalAttendance = presentDays;

            return result;
        }

        private string GetEmployeeCodeById(int serverId, string databaseName, int employeeId)
        {
            string connString = GetConnectionString(serverId, databaseName);

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                string query = @"SELECT EmployeeId FROM dbo.Employees WHERE Id = @EmployeeId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeId", employeeId);

                    object value = cmd.ExecuteScalar();
                    return value == null || value == DBNull.Value ? "" : value.ToString();
                }
            }
        }

        private bool HasColumn(int serverId, string databaseName, string tableName, string columnName)
        {
            string connString = GetConnectionString(serverId, databaseName);

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                string query = @"SELECT COL_LENGTH(@TableName, @ColumnName)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TableName", tableName);
                    cmd.Parameters.AddWithValue("@ColumnName", columnName);

                    object value = cmd.ExecuteScalar();
                    return value != DBNull.Value && value != null;
                }
            }
        }

        private string GetConnectionString(int serverId, string databaseName)
        {
            string serverIp = GetServerIp(serverId);
            string username = "sa";
            string password = "open";

            return $"Server={serverIp};Database={databaseName};User Id={username};Password={password};TrustServerCertificate=True;";
        }

        private string GetServerIp(int serverId)
        {
            return "192.168.26.242";
        }

        private class ManualLoginRow
        {
            public DateTime AttendanceDate { get; set; }
            public DateTime? LoginTime { get; set; }
            public DateTime? OutTime { get; set; }
        }
    }
}