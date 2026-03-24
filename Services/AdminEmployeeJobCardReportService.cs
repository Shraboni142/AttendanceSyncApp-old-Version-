using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using AttendanceSyncApp.Models.DTOs.Reports;
using AttendanceSyncApp.Services.Interfaces;

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

                string employeeCode = "";

                string employeeQuery = @"
            SELECT EmployeeId
            FROM dbo.Employees
            WHERE Id = @Id";

                using (SqlCommand employeeCmd = new SqlCommand(employeeQuery, conn))
                {
                    employeeCmd.Parameters.AddWithValue("@Id", employeeId);

                    object employeeCodeObj = employeeCmd.ExecuteScalar();
                    employeeCode = employeeCodeObj == null || employeeCodeObj == DBNull.Value
                        ? ""
                        : employeeCodeObj.ToString().Trim();
                }

                var presentDates = new HashSet<DateTime>();

                string attendanceQuery = @"
            SELECT DISTINCT
                CAST([Date] AS date) AS AttendanceDate
            FROM dbo.EmployeeManualLogins
            WHERE TRY_CAST(EmployeeId AS INT) = TRY_CAST(@EmployeeCode AS INT)
              AND LoginTime IS NOT NULL
              AND CAST([Date] AS date) BETWEEN @FromDate AND @ToDate";

                using (SqlCommand attendanceCmd = new SqlCommand(attendanceQuery, conn))
                {
                    attendanceCmd.Parameters.AddWithValue("@EmployeeCode", employeeCode);
                    attendanceCmd.Parameters.AddWithValue("@FromDate", Convert.ToDateTime(fromDate).Date);
                    attendanceCmd.Parameters.AddWithValue("@ToDate", Convert.ToDateTime(toDate).Date);

                    using (SqlDataReader reader = attendanceCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["AttendanceDate"] != DBNull.Value)
                            {
                                presentDates.Add(Convert.ToDateTime(reader["AttendanceDate"]).Date);
                            }
                        }
                    }
                }

                DateTime startDate = Convert.ToDateTime(fromDate).Date;
                DateTime endDate = Convert.ToDateTime(toDate).Date;

                for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    bool isPresent = presentDates.Contains(date.Date);

                    result.Add(new EmployeeJobCardDetailDto
                    {
                        Date = date.ToString("dd-MMM-yyyy"),
                        Shift = "",
                        InTime = "",
                        OutTime = "",
                        LateMinutes = "",
                        OTHours = "",
                        DayStatus = isPresent ? "Present" : "Absent",
                        Remarks = ""
                    });
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