using AttendanceSyncApp.Models.DTOs.ComplianceAction;
using AttendanceSyncApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace AttendanceSyncApp.Services
{
    public class AdminComplianceActionService : IAdminComplianceActionService
    {
        private readonly string _connectionString;

        public AdminComplianceActionService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["AttandanceSyncConnection"].ConnectionString;
        }

        public bool SaveComplianceAction(ComplianceActionCreateDto dto)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
INSERT INTO dbo.EmployeeComplianceActions
(
    EmployeeId,
    EmployeeCode,
    EmployeeName,
    OffenceType,
    OffenceDetails,
    ComplianceActionType,
    ComplianceActionDetails,
    DateOfNotice,
    FromDate,
    ToDate,
    EarlyWithdrawalDate,
    AttachmentFileName,
    AttachmentFilePath,
    IsActive,
    CreatedBy,
    CreatedAt
)
VALUES
(
    @EmployeeId,
    @EmployeeCode,
    @EmployeeName,
    @OffenceType,
    @OffenceDetails,
    @ComplianceActionType,
    @ComplianceActionDetails,
    @DateOfNotice,
    @FromDate,
    @ToDate,
    @EarlyWithdrawalDate,
    @AttachmentFileName,
    @AttachmentFilePath,
    1,
    @CreatedBy,
    GETDATE()
)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    
                    cmd.Parameters.AddWithValue("@EmployeeId", (object)dto.EmployeeId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@EmployeeCode", (object)dto.EmployeeCode ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@EmployeeName", (object)dto.EmployeeName ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@OffenceType", dto.OffenceType ?? "");
                    cmd.Parameters.AddWithValue("@OffenceDetails", (object)dto.OffenceDetails ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@ComplianceActionType", dto.ComplianceActionType ?? "");
                    cmd.Parameters.AddWithValue("@ComplianceActionDetails", (object)dto.ComplianceActionDetails ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@DateOfNotice", dto.DateOfNotice);
                    cmd.Parameters.AddWithValue("@FromDate", (object)dto.FromDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ToDate", (object)dto.ToDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@EarlyWithdrawalDate", (object)dto.EarlyWithdrawalDate ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@AttachmentFileName", (object)dto.AttachmentFileName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AttachmentFilePath", (object)dto.AttachmentFilePath ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@CreatedBy", (object)dto.CreatedBy ?? DBNull.Value);

                    conn.Open();
                    int rowAffected = cmd.ExecuteNonQuery();
                    return rowAffected > 0;
                }
            }
        }
        public List<ComplianceActionListDto> GetAllComplianceActions()
        {
            List<ComplianceActionListDto> list = new List<ComplianceActionListDto>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
SELECT 
    Id,
    EmployeeId,
    EmployeeCode,
    EmployeeName,
    OffenceType,
    OffenceDetails,
    ComplianceActionType,
    ComplianceActionDetails
FROM dbo.EmployeeComplianceActions
ORDER BY Id DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new ComplianceActionListDto
                            {
                                Id = reader["Id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Id"]),
                                EmployeeId = reader["EmployeeId"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["EmployeeId"]),
                                EmployeeCode = reader["EmployeeCode"] == DBNull.Value ? "" : reader["EmployeeCode"].ToString(),
                                EmployeeName = reader["EmployeeName"] == DBNull.Value ? "" : reader["EmployeeName"].ToString(),
                                OffenceType = reader["OffenceType"] == DBNull.Value ? "" : reader["OffenceType"].ToString(),
                                OffenceDetails = reader["OffenceDetails"] == DBNull.Value ? "" : reader["OffenceDetails"].ToString(),
                                ComplianceActionType = reader["ComplianceActionType"] == DBNull.Value ? "" : reader["ComplianceActionType"].ToString(),
                                ComplianceActionDetails = reader["ComplianceActionDetails"] == DBNull.Value ? "" : reader["ComplianceActionDetails"].ToString()
                            });
                        }
                    }
                }
            }

            return list;
        }
        public ComplianceActionCreateDto GetComplianceActionById(int id)
        {
            ComplianceActionCreateDto dto = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
SELECT 
    Id,
    EmployeeId,
    EmployeeCode,
    EmployeeName,
    OffenceType,
    OffenceDetails,
    ComplianceActionType,
    ComplianceActionDetails,
    DateOfNotice,
    FromDate,
    ToDate,
    EarlyWithdrawalDate,
    AttachmentFileName,
    AttachmentFilePath
FROM dbo.EmployeeComplianceActions
WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            dto = new ComplianceActionCreateDto
                            {
                                Id = reader["Id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Id"]),
                                EmployeeId = reader["EmployeeId"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["EmployeeId"]),
                                EmployeeCode = reader["EmployeeCode"] == DBNull.Value ? "" : reader["EmployeeCode"].ToString(),
                                EmployeeName = reader["EmployeeName"] == DBNull.Value ? "" : reader["EmployeeName"].ToString(),
                                OffenceType = reader["OffenceType"] == DBNull.Value ? "" : reader["OffenceType"].ToString(),
                                OffenceDetails = reader["OffenceDetails"] == DBNull.Value ? "" : reader["OffenceDetails"].ToString(),
                                ComplianceActionType = reader["ComplianceActionType"] == DBNull.Value ? "" : reader["ComplianceActionType"].ToString(),
                                ComplianceActionDetails = reader["ComplianceActionDetails"] == DBNull.Value ? "" : reader["ComplianceActionDetails"].ToString(),
                                DateOfNotice = reader["DateOfNotice"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["DateOfNotice"]),
                                FromDate = reader["FromDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["FromDate"]),
                                ToDate = reader["ToDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["ToDate"]),
                                EarlyWithdrawalDate = reader["EarlyWithdrawalDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["EarlyWithdrawalDate"]),
                                AttachmentFileName = reader["AttachmentFileName"] == DBNull.Value ? "" : reader["AttachmentFileName"].ToString(),
                                AttachmentFilePath = reader["AttachmentFilePath"] == DBNull.Value ? "" : reader["AttachmentFilePath"].ToString()
                            };
                        }
                    }
                }
            }

            return dto;
        }
        public bool UpdateComplianceAction(ComplianceActionCreateDto dto)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
UPDATE dbo.EmployeeComplianceActions
SET
    EmployeeId = @EmployeeId,
    EmployeeCode = @EmployeeCode,
    EmployeeName = @EmployeeName,
    OffenceType = @OffenceType,
    OffenceDetails = @OffenceDetails,
    ComplianceActionType = @ComplianceActionType,
    ComplianceActionDetails = @ComplianceActionDetails,
    DateOfNotice = @DateOfNotice,
    FromDate = @FromDate,
    ToDate = @ToDate,
    EarlyWithdrawalDate = @EarlyWithdrawalDate,
    AttachmentFileName = @AttachmentFileName,
    AttachmentFilePath = @AttachmentFilePath,
    UpdatedBy = @UpdatedBy,
    UpdatedAt = GETDATE()
WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", dto.Id);
                    cmd.Parameters.AddWithValue("@EmployeeId", (object)dto.EmployeeId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@EmployeeCode", (object)dto.EmployeeCode ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@EmployeeName", (object)dto.EmployeeName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@OffenceType", dto.OffenceType ?? "");
                    cmd.Parameters.AddWithValue("@OffenceDetails", (object)dto.OffenceDetails ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ComplianceActionType", dto.ComplianceActionType ?? "");
                    cmd.Parameters.AddWithValue("@ComplianceActionDetails", (object)dto.ComplianceActionDetails ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DateOfNotice", dto.DateOfNotice);
                    cmd.Parameters.AddWithValue("@FromDate", (object)dto.FromDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ToDate", (object)dto.ToDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@EarlyWithdrawalDate", (object)dto.EarlyWithdrawalDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AttachmentFileName", (object)dto.AttachmentFileName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AttachmentFilePath", (object)dto.AttachmentFilePath ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@UpdatedBy", (object)dto.CreatedBy ?? DBNull.Value);

                    conn.Open();
                    int rowAffected = cmd.ExecuteNonQuery();
                    return rowAffected > 0;
                }
            }
        }
        public bool DeleteComplianceAction(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM dbo.EmployeeComplianceActions WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    conn.Open();
                    int rowAffected = cmd.ExecuteNonQuery();
                    return rowAffected > 0;
                }
            }
        }
    }
}