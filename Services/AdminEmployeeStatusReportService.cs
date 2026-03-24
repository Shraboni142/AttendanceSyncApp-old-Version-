using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using AttendanceSyncApp.Models.DTOs.Reports;
using AttendanceSyncApp.Services.Interfaces;

namespace AttendanceSyncApp.Services
{
    public class AdminEmployeeStatusReportService : IAdminEmployeeStatusReportService
    {
        public List<EmployeeStatusReportRowDto> GetEmployeeStatusReport(
            int serverId,
            List<string> databases,
            int status)
        {
            var result = new List<EmployeeStatusReportRowDto>();

            string serverIp = GetServerIp(serverId);
            string username = "sa";
            string password = "open";

            if (databases == null || databases.Count == 0)
            {
                return result;
            }

            foreach (var db in databases)
            {
                string connString =
                    $"Server={serverIp};Database={db};User Id={username};Password={password};TrustServerCertificate=True;";

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    string query = @"
                        SELECT
                            e.EmployeeId,
                            LTRIM(RTRIM(
                                ISNULL(e.FirstName, '') + ' ' +
                                ISNULL(e.MiddleName, '') + ' ' +
                                ISNULL(e.LastName, '')
                            )) AS EmployeeName,
                            b.BranchCode,
                            b.BranchName,
                            d.DesignationName,
                            e.BasicSalary,
                            e.Email,
                            e.MobileNo,
                            e.IsActive
                       FROM dbo.Employees e
LEFT JOIN dbo.Branches b ON e.BranchId = b.Id
LEFT JOIN dbo.Designations d ON e.DesignationId = d.Id
LEFT JOIN dbo.Sections s ON e.SectionId = s.Id
LEFT JOIN dbo.Locations l ON e.LocationId = l.Id

                        WHERE e.IsActive = @Status
                        ORDER BY e.EmployeeId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Status", status);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(new EmployeeStatusReportRowDto
                                {
                                    EmployeeId = reader["EmployeeId"] == DBNull.Value ? "" : reader["EmployeeId"].ToString(),
                                    EmployeeName = reader["EmployeeName"] == DBNull.Value ? "" : reader["EmployeeName"].ToString(),
                                    BranchCode = reader["BranchCode"] == DBNull.Value ? "" : reader["BranchCode"].ToString(),
                                    BranchName = reader["BranchName"] == DBNull.Value ? "" : reader["BranchName"].ToString(),
                                    DesignationName = reader["DesignationName"] == DBNull.Value ? "" : reader["DesignationName"].ToString(),
                                    BasicSalary = reader["BasicSalary"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["BasicSalary"]),
                                    Email = reader["Email"] == DBNull.Value ? "" : reader["Email"].ToString(),
                                    MobileNo = reader["MobileNo"] == DBNull.Value ? "" : reader["MobileNo"].ToString(),
                                    IsActive = reader["IsActive"] != DBNull.Value && Convert.ToBoolean(reader["IsActive"])
                                });
                            }
                        }
                    }
                }
            }

            return result;
        }

        private string GetServerIp(int serverId)
        {
            return "192.168.26.242";
        }
    }
}