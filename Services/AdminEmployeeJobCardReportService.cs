using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using AttendanceSyncApp.Models.DTOs.Reports;
using AttendanceSyncApp.Services.Interfaces;

namespace AttendanceSyncApp.Services
{
    public class AdminEmployeeJobCardReportService : IAdminEmployeeJobCardReportService
    {
        public List<EmployeeDropdownDto> GetEmployees(int serverId, string databaseName)
        {
            var result = new List<EmployeeDropdownDto>();

            string serverIp = GetServerIp(serverId);
            string username = "sa";
            string password = "open";

            string connString =
                $"Server={serverIp};Database={databaseName};User Id={username};Password={password};TrustServerCertificate=True;";

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
            return new EmployeeJobCardHeaderDto();
        }

        public List<EmployeeJobCardDetailDto> GetDetailData(int serverId, string databaseName, int employeeId, string fromDate, string toDate)
        {
            return new List<EmployeeJobCardDetailDto>();
        }

        public EmployeeJobCardSummaryDto GetSummaryData(List<EmployeeJobCardDetailDto> details)
        {
            return new EmployeeJobCardSummaryDto();
        }

        private string GetServerIp(int serverId)
        {
            return "192.168.26.242";
        }
    }
}