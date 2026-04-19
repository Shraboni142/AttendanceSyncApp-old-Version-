using AttendanceSyncApp.Models.DTOs.ComplainAction;
using AttendanceSyncApp.Services.Interfaces;
using Org.BouncyCastle.Asn1.X509;
using SkiaSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace AttendanceSyncApp.Services
{
    public class AdminComplainActionService : IAdminComplainActionService
    {
        private readonly string _connectionString;

        public AdminComplainActionService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["AttandanceSyncConnection"].ConnectionString;
        }

        public bool SaveComplainAction(ComplainActionCreateDto dto)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
INSERT INTO dbo.EmployeeComplainActions
(
    EmployeeCode,
    EmployeeName,
    OffenceType,
    OffenceDetails,
    ComplainActionType,
    ComplainActionDetails,
    DateOfNotice,
    EarlyWithdrawalDate,
    AttachmentFileName,
    AttachmentFilePath,
    ReviewStatus,
    IsActive,
    CreatedBy,
    CreatedAt
)
VALUES
(
    @EmployeeCode,
    @EmployeeName,
    @OffenceType,
    @OffenceDetails,
    @ComplainActionType,
    @ComplainActionDetails,
    @DateOfNotice,
    @EarlyWithdrawalDate,
    @AttachmentFileName,
    @AttachmentFilePath,
    @ReviewStatus,
    1,
    @CreatedBy,
    GETDATE()
)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeCode", (object)dto.EmployeeCode ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@EmployeeName", (object)dto.EmployeeName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@OffenceType", dto.OffenceType ?? "");
                    cmd.Parameters.AddWithValue("@OffenceDetails", (object)dto.OffenceDetails ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ComplainActionType", dto.ComplainActionType ?? "");
                    cmd.Parameters.AddWithValue("@ComplainActionDetails", (object)dto.ComplainActionDetails ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DateOfNotice", dto.DateOfNotice);
                    cmd.Parameters.AddWithValue("@EarlyWithdrawalDate", (object)dto.EarlyWithdrawalDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AttachmentFileName", (object)dto.AttachmentFileName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AttachmentFilePath", (object)dto.AttachmentFilePath ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ReviewStatus",
    string.IsNullOrWhiteSpace(dto.ReviewStatus) ? "Pending" : dto.ReviewStatus);
                    cmd.Parameters.AddWithValue("@CreatedBy", (object)dto.CreatedBy ?? DBNull.Value);

                    conn.Open();
                    int rowAffected = cmd.ExecuteNonQuery();
                    return rowAffected > 0;
                }
            }
        }

        public List<ComplainActionListDto> GetAllComplainActions()
        {
            List<ComplainActionListDto> list = new List<ComplainActionListDto>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
string query = @"
; WITH LatestEmployeeComplain AS
(
    SELECT
        Id,
        EmployeeCode,
        EmployeeName,
        OffenceType,
        OffenceDetails,
        ComplainActionType,
        ComplainActionDetails,
        ReviewStatus,
        ROW_NUMBER() OVER(PARTITION BY EmployeeCode ORDER BY Id DESC) AS RowNum
    FROM dbo.EmployeeComplainActions
)
SELECT
    Id,
    EmployeeCode,
    EmployeeName,
    OffenceType,
    OffenceDetails,
    ComplainActionType,
    ComplainActionDetails,
    ReviewStatus
FROM LatestEmployeeComplain
WHERE RowNum = 1
ORDER BY Id DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new ComplainActionListDto
                            {
                                Id = reader["Id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Id"]),
                                EmployeeCode = reader["EmployeeCode"] == DBNull.Value ? "" : reader["EmployeeCode"].ToString(),
                                EmployeeName = reader["EmployeeName"] == DBNull.Value ? "" : reader["EmployeeName"].ToString(),
                                OffenceType = reader["OffenceType"] == DBNull.Value ? "" : reader["OffenceType"].ToString(),
                                OffenceDetails = reader["OffenceDetails"] == DBNull.Value ? "" : reader["OffenceDetails"].ToString(),
                                ComplainActionType = reader["ComplainActionType"] == DBNull.Value ? "" : reader["ComplainActionType"].ToString(),
                                ComplainActionDetails = reader["ComplainActionDetails"] == DBNull.Value ? "" : reader["ComplainActionDetails"].ToString(),
                                ReviewStatus = reader["ReviewStatus"] == DBNull.Value ? "" : reader["ReviewStatus"].ToString()
                            });
                        }
                    }
                }
            }

            return list;
        }

        public ComplainActionCreateDto GetComplainActionById(int id)
        {
            ComplainActionCreateDto dto = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
SELECT 
    Id,
    EmployeeCode,
    EmployeeName,
    OffenceType,
    OffenceDetails,
    ComplainActionType,
    ComplainActionDetails,
    DateOfNotice,
    EarlyWithdrawalDate,
    AttachmentFileName,
    AttachmentFilePath,
    ReviewStatus
FROM dbo.EmployeeComplainActions
WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            dto = new ComplainActionCreateDto
                            {
                                Id = reader["Id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Id"]),
                                EmployeeCode = reader["EmployeeCode"] == DBNull.Value ? "" : reader["EmployeeCode"].ToString(),
                                EmployeeName = reader["EmployeeName"] == DBNull.Value ? "" : reader["EmployeeName"].ToString(),
                                OffenceType = reader["OffenceType"] == DBNull.Value ? "" : reader["OffenceType"].ToString(),
                                OffenceDetails = reader["OffenceDetails"] == DBNull.Value ? "" : reader["OffenceDetails"].ToString(),
                                ComplainActionType = reader["ComplainActionType"] == DBNull.Value ? "" : reader["ComplainActionType"].ToString(),
                                ComplainActionDetails = reader["ComplainActionDetails"] == DBNull.Value ? "" : reader["ComplainActionDetails"].ToString(),
                                DateOfNotice = reader["DateOfNotice"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["DateOfNotice"]),
                                EarlyWithdrawalDate = reader["EarlyWithdrawalDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["EarlyWithdrawalDate"]),
                                AttachmentFileName = reader["AttachmentFileName"] == DBNull.Value ? "" : reader["AttachmentFileName"].ToString(),
                                AttachmentFilePath = reader["AttachmentFilePath"] == DBNull.Value ? "" : reader["AttachmentFilePath"].ToString(),
                                ReviewStatus = reader["ReviewStatus"] == DBNull.Value ? "" : reader["ReviewStatus"].ToString()
                            };
                        }
                    }
                }
            }

            return dto;
        }

        public bool UpdateComplainAction(ComplainActionCreateDto dto)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                if (string.IsNullOrWhiteSpace(dto.AttachmentFileName) || string.IsNullOrWhiteSpace(dto.AttachmentFilePath))
                {
                    string previousQuery = "SELECT AttachmentFileName, AttachmentFilePath FROM dbo.EmployeeComplainActions WHERE Id = @Id";

                    using (SqlCommand previousCmd = new SqlCommand(previousQuery, conn))
                    {
                        previousCmd.Parameters.AddWithValue("@Id", dto.Id);

                        conn.Open();
                        using (SqlDataReader reader = previousCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (string.IsNullOrWhiteSpace(dto.AttachmentFileName))
                                {
                                    dto.AttachmentFileName = reader["AttachmentFileName"] == DBNull.Value ? "" : reader["AttachmentFileName"].ToString();
                                }

                                if (string.IsNullOrWhiteSpace(dto.AttachmentFilePath))
                                {
                                    dto.AttachmentFilePath = reader["AttachmentFilePath"] == DBNull.Value ? "" : reader["AttachmentFilePath"].ToString();
                                }
                            }
                        }
                        conn.Close();
                    }
                }

                string query = @"
UPDATE dbo.EmployeeComplainActions
SET
    EmployeeCode = @EmployeeCode,
    EmployeeName = @EmployeeName,
    OffenceType = @OffenceType,
    OffenceDetails = @OffenceDetails,
    ComplainActionType = @ComplainActionType,
    ComplainActionDetails = @ComplainActionDetails,
    DateOfNotice = @DateOfNotice,
    EarlyWithdrawalDate = @EarlyWithdrawalDate,
    AttachmentFileName = @AttachmentFileName,
    AttachmentFilePath = @AttachmentFilePath,
    ReviewStatus = @ReviewStatus,
    UpdatedBy = @UpdatedBy,
    UpdatedAt = GETDATE()
WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", dto.Id);
                    cmd.Parameters.AddWithValue("@EmployeeCode", (object)dto.EmployeeCode ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@EmployeeName", (object)dto.EmployeeName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@OffenceType", dto.OffenceType ?? "");
                    cmd.Parameters.AddWithValue("@OffenceDetails", (object)dto.OffenceDetails ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ComplainActionType", dto.ComplainActionType ?? "");
                    cmd.Parameters.AddWithValue("@ComplainActionDetails", (object)dto.ComplainActionDetails ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DateOfNotice", dto.DateOfNotice);
                    cmd.Parameters.AddWithValue("@EarlyWithdrawalDate", (object)dto.EarlyWithdrawalDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AttachmentFileName", (object)dto.AttachmentFileName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AttachmentFilePath", (object)dto.AttachmentFilePath ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ReviewStatus",
                        string.IsNullOrWhiteSpace(dto.ReviewStatus) ? "Pending" : dto.ReviewStatus);
                    cmd.Parameters.AddWithValue("@UpdatedBy", (object)dto.CreatedBy ?? DBNull.Value);

                    conn.Open();
                    int rowAffected = cmd.ExecuteNonQuery();
                    return rowAffected > 0;
                }
            }
        }

        public bool DeleteComplainAction(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM dbo.EmployeeComplainActions WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    conn.Open();
                    int rowAffected = cmd.ExecuteNonQuery();
                    return rowAffected > 0;
                }
            }
        }

        public bool UpdateComplainReviewStatus(int id, string reviewStatus)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
UPDATE dbo.EmployeeComplainActions
SET
    ReviewStatus = @ReviewStatus,
    UpdatedAt = GETDATE()
WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@ReviewStatus", (object)reviewStatus ?? DBNull.Value);

                    conn.Open();
                    int rowAffected = cmd.ExecuteNonQuery();
                    return rowAffected > 0;
                }
            }
        }
    }
}