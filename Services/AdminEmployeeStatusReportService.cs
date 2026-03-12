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
            if (databases == null)
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
                        @DatabaseName AS DatabaseName,
                        Id,
                        BranchId,
                        GradeScaleId,
                        MobileNo,
                        DesignationId,
                        BasicSalary
                    FROM dbo.Employees
                    WHERE IsActive = @Status";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@DatabaseName", db);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        result.Add(new EmployeeStatusReportRowDto
                        {
                            DatabaseName = reader["DatabaseName"].ToString(),
                            Id = Convert.ToInt32(reader["Id"]),
                            BranchId = reader["BranchId"] as int?,
                            GradeScaleId = reader["GradeScaleId"] as int?,
                            MobileNo = reader["MobileNo"].ToString(),
                            DesignationId = reader["DesignationId"] as int?,
                            BasicSalary = reader["BasicSalary"] as decimal?
                        });
                    }

                    reader.Close();
                }
            }

            return result;
        }

        private string GetServerIp(int serverId)
        {
            // ekhane existing ServerIps table theke ip nite hobe
            return "192.168.26.242";
        }
    }
}