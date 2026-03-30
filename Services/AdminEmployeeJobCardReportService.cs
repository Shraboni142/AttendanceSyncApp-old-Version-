using AttandanceSyncApp.Helpers;
using AttandanceSyncApp.Repositories;
using AttendanceSyncApp.Models.DTOs.Reports;
using AttendanceSyncApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
                    cmd.Parameters.Add("@EmployeeId", SqlDbType.Int).Value = employeeId;

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

        public List<BranchDropdownDto> GetBranches(int serverId, string databaseName)
        {
            var result = new List<BranchDropdownDto>();

            string connString = GetConnectionString(serverId, databaseName);

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                string query = @"
            SELECT
                Id,
                BranchName
            FROM dbo.Branches
            ORDER BY BranchName";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new BranchDropdownDto
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            BranchName = reader["BranchName"] == DBNull.Value ? "" : reader["BranchName"].ToString()
                        });
                    }
                }
            }

            return result;
        }

        public List<EmployeeDropdownDto> GetEmployeesByBranchAndStatus(int serverId, string databaseName, int branchId, int status)
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
            WHERE BranchId = @BranchId
              AND IsActive = @Status
            ORDER BY EmployeeId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@BranchId", SqlDbType.Int).Value = branchId;
                    cmd.Parameters.Add("@Status", SqlDbType.Int).Value = status;

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

            string connString = GetConnectionString(serverId, databaseName);

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                string query = @"
;WITH EmployeeInfo AS
(
    SELECT 
        Id AS InternalEmployeeId,
        LTRIM(RTRIM(CAST(EmployeeId AS NVARCHAR(50)))) AS EmployeeCode
    FROM dbo.Employees
    WHERE Id = @EmployeeId
),
DateRange AS
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
        CAST(eml.[Date] AS DATE) AS AttendanceDate,
        MIN(eml.LoginTime) AS InTime,
        MAX(eml.LogoutTime) AS OutTime
    FROM dbo.EmployeeManualLogins eml
    CROSS JOIN EmployeeInfo ei
    WHERE CAST(eml.[Date] AS DATE) BETWEEN CAST(@FromDate AS DATE) AND CAST(@ToDate AS DATE)
      AND eml.LoginTime IS NOT NULL
      AND
      (
          LTRIM(RTRIM(CAST(eml.EmployeeId AS NVARCHAR(50)))) = LTRIM(RTRIM(CAST(ei.InternalEmployeeId AS NVARCHAR(50))))
          OR
          LTRIM(RTRIM(CAST(eml.EmployeeId AS NVARCHAR(50)))) = ei.EmployeeCode
      )
    GROUP BY CAST(eml.[Date] AS DATE)
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

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@EmployeeId", SqlDbType.Int).Value = employeeId;
                    cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = Convert.ToDateTime(fromDate).Date;
                    cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = Convert.ToDateTime(toDate).Date;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new EmployeeJobCardDetailDto
                            {
                                Date = Convert.ToDateTime(reader["Date"]).ToString("dd-MMM-yyyy"),
                                Shift = reader["Shift"] == DBNull.Value ? "" : reader["Shift"].ToString(),
                                InTime = reader["InTime"] == DBNull.Value ? "" : Convert.ToDateTime(reader["InTime"]).ToString("hh:mm tt"),
                                OutTime = reader["OutTime"] == DBNull.Value ? "" : Convert.ToDateTime(reader["OutTime"]).ToString("hh:mm tt"),
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

        public EmployeeJobCardDayDetailDto GetDayDetailReport(int serverId, string databaseName, int employeeId, string selectedDate)
        {
            EmployeeJobCardDayDetailDto model = null;

            string connString = GetConnectionString(serverId, databaseName);

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                string query = @"
SELECT TOP 1
    c.CompanyName,
    e.EmployeeId AS EmployeeCode,
    LTRIM(RTRIM(
        ISNULL(e.FirstName, '') + ' ' +
        ISNULL(e.MiddleName, '') + ' ' +
        ISNULL(e.LastName, '')
    )) AS EmployeeName,
    d.DesignationName,
    dp.DepartmentName,
    b.BranchName,
    s.SectionName,
    l.LocationName AS Location,
    CONVERT(VARCHAR(20), e.JoiningDate, 105) AS DOJ,
    CONVERT(VARCHAR(20), @SelectedDate, 106) AS AttendanceDate,
    CASE
        WHEN eml.LoginTime IS NOT NULL THEN 'Present'
        ELSE 'Absent'
    END AS DayStatus,
    CONVERT(VARCHAR(20), eml.LoginTime, 100) AS LoginTime,
    CONVERT(VARCHAR(20), eml.LogoutTime, 100) AS LogoutTime,
    CONVERT(VARCHAR(20), eml.ExpectedLoginTime, 100) AS ExpectedLoginTime,
    CONVERT(VARCHAR(20), eml.ExpectedLogoutTime, 100) AS ExpectedLogoutTime,
    CAST(eml.LoginworkstationId AS VARCHAR(50)) AS LoginWorkstationId,
    CAST(eml.LogoutworkstationId AS VARCHAR(50)) AS LogoutWorkstationId,
    eml.Processcode,
    CAST(eml.BranchId AS VARCHAR(50)) AS BranchId,
    CASE WHEN eml.Ismanuallogin = 1 THEN 'Yes' ELSE 'No' END AS IsManualLogin,
    CASE WHEN eml.IsLate = 1 THEN 'Yes' ELSE 'No' END AS IsLate,
    CONVERT(VARCHAR(20), eml.AbsentCountingTime, 100) AS AbsentCountingTime,
    CASE
        WHEN eml.LoginTime IS NOT NULL AND eml.LogoutTime IS NOT NULL
        THEN CAST(DATEDIFF(MINUTE, eml.LoginTime, eml.LogoutTime) / 60 AS VARCHAR(10))
             + ' Hr '
             + CAST(DATEDIFF(MINUTE, eml.LoginTime, eml.LogoutTime) % 60 AS VARCHAR(10))
             + ' Min'
        ELSE ''
    END AS WorkingHours,
    CASE
        WHEN eml.LoginTime IS NOT NULL THEN 'Login record found'
        ELSE 'No login record found for selected date'
    END AS Remarks
FROM dbo.Employees e
LEFT JOIN dbo.Designations d ON e.DesignationId = d.Id
LEFT JOIN dbo.Departments dp ON e.DepartmentId = dp.Id
LEFT JOIN dbo.Branches b ON e.BranchId = b.Id
LEFT JOIN dbo.Sections s ON e.SectionId = s.Id
LEFT JOIN dbo.Locations l ON e.LocationId = l.Id
LEFT JOIN dbo.Companies c ON e.CompanyId = c.Id
LEFT JOIN dbo.EmployeeManualLogins eml
       ON LTRIM(RTRIM(CAST(eml.EmployeeId AS NVARCHAR(50)))) = LTRIM(RTRIM(CAST(e.EmployeeId AS NVARCHAR(50))))
      AND CAST(eml.[Date] AS DATE) = CAST(@SelectedDate AS DATE)
WHERE e.Id = @EmployeeId;";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@EmployeeId", SqlDbType.Int).Value = employeeId;
                    cmd.Parameters.Add("@SelectedDate", SqlDbType.Date).Value = Convert.ToDateTime(selectedDate).Date;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            model = new EmployeeJobCardDayDetailDto
                            {
                                CompanyName = reader["CompanyName"] == DBNull.Value ? "" : reader["CompanyName"].ToString(),
                                EmployeeCode = reader["EmployeeCode"] == DBNull.Value ? "" : reader["EmployeeCode"].ToString(),
                                EmployeeName = reader["EmployeeName"] == DBNull.Value ? "" : reader["EmployeeName"].ToString(),
                                Designation = reader["DesignationName"] == DBNull.Value ? "" : reader["DesignationName"].ToString(),
                                Department = reader["DepartmentName"] == DBNull.Value ? "" : reader["DepartmentName"].ToString(),
                                Branch = reader["BranchName"] == DBNull.Value ? "" : reader["BranchName"].ToString(),
                                Section = reader["SectionName"] == DBNull.Value ? "" : reader["SectionName"].ToString(),
                                Location = reader["Location"] == DBNull.Value ? "" : reader["Location"].ToString(),
                                DOJ = reader["DOJ"] == DBNull.Value ? "" : reader["DOJ"].ToString(),
                                AttendanceDate = reader["AttendanceDate"] == DBNull.Value ? "" : reader["AttendanceDate"].ToString(),
                                DayStatus = reader["DayStatus"] == DBNull.Value ? "" : reader["DayStatus"].ToString(),
                                LoginTime = reader["LoginTime"] == DBNull.Value ? "" : reader["LoginTime"].ToString(),
                                LogoutTime = reader["LogoutTime"] == DBNull.Value ? "" : reader["LogoutTime"].ToString(),
                                ExpectedLoginTime = reader["ExpectedLoginTime"] == DBNull.Value ? "" : reader["ExpectedLoginTime"].ToString(),
                                ExpectedLogoutTime = reader["ExpectedLogoutTime"] == DBNull.Value ? "" : reader["ExpectedLogoutTime"].ToString(),
                                LoginWorkstationId = reader["LoginWorkstationId"] == DBNull.Value ? "" : reader["LoginWorkstationId"].ToString(),
                                LogoutWorkstationId = reader["LogoutWorkstationId"] == DBNull.Value ? "" : reader["LogoutWorkstationId"].ToString(),
                                Processcode = reader["Processcode"] == DBNull.Value ? "" : reader["Processcode"].ToString(),
                                BranchId = reader["BranchId"] == DBNull.Value ? "" : reader["BranchId"].ToString(),
                                IsManualLogin = reader["IsManualLogin"] == DBNull.Value ? "" : reader["IsManualLogin"].ToString(),
                                IsLate = reader["IsLate"] == DBNull.Value ? "" : reader["IsLate"].ToString(),
                                AbsentCountingTime = reader["AbsentCountingTime"] == DBNull.Value ? "" : reader["AbsentCountingTime"].ToString(),
                                WorkingHours = reader["WorkingHours"] == DBNull.Value ? "" : reader["WorkingHours"].ToString(),
                                Remarks = reader["Remarks"] == DBNull.Value ? "" : reader["Remarks"].ToString()
                            };
                        }
                    }
                }
            }

            return model;
        }

        private string GetConnectionString(int serverId, string databaseName)
        {
            using (var unitOfWork = new AuthUnitOfWork())
            {
                var server = unitOfWork.ServerIps.GetById(serverId);

                if (server == null || !server.IsActive)
                {
                    throw new Exception("Active server information not found.");
                }

                string decryptedPassword = EncryptionHelper.Decrypt(server.DatabasePassword);

                return $"Server={server.IpAddress};Database={databaseName};User Id={server.DatabaseUser};Password={decryptedPassword};TrustServerCertificate=True;";
            }
        }
    }
}