using AttendanceSyncApp.Models.DTOs.GpsSystem;
using AttendanceSyncApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace AttendanceSyncApp.Services
{
    public class GpsSystemService : IGpsSystemService
    {
        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["AttandanceSyncConnection"].ConnectionString;
        }

        public List<GpsTrackerUserDto> GetTrackerUsers()
        {
            var list = new List<GpsTrackerUserDto>();

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = @"
                    SELECT 
                        Id,
                        UserId,
                        EmployeeCode,
                        EmployeeName,
                        MobileNo,
                        DepartmentName,
                        BranchName,
                        IsActive,
                        IsLiveTrackingEnabled,
                        IsFieldVisitEnabled,
                        CreatedAt
                    FROM dbo.GpsTrackerUsers
                    ORDER BY Id DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new GpsTrackerUserDto
                        {
                            Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0,
                            UserId = reader["UserId"] != DBNull.Value ? (int?)Convert.ToInt32(reader["UserId"]) : null,
                            EmployeeCode = reader["EmployeeCode"] != DBNull.Value ? reader["EmployeeCode"].ToString() : "",
                            EmployeeName = reader["EmployeeName"] != DBNull.Value ? reader["EmployeeName"].ToString() : "",
                            MobileNo = reader["MobileNo"] != DBNull.Value ? reader["MobileNo"].ToString() : "",
                            DepartmentName = reader["DepartmentName"] != DBNull.Value ? reader["DepartmentName"].ToString() : "",
                            BranchName = reader["BranchName"] != DBNull.Value ? reader["BranchName"].ToString() : "",
                            IsActive = reader["IsActive"] != DBNull.Value && Convert.ToBoolean(reader["IsActive"]),
                            IsLiveTrackingEnabled = reader["IsLiveTrackingEnabled"] != DBNull.Value && Convert.ToBoolean(reader["IsLiveTrackingEnabled"]),
                            IsFieldVisitEnabled = reader["IsFieldVisitEnabled"] != DBNull.Value && Convert.ToBoolean(reader["IsFieldVisitEnabled"]),
                            CreatedAtText = reader["CreatedAt"] != DBNull.Value
                                ? Convert.ToDateTime(reader["CreatedAt"]).ToString("dd-MMM-yyyy hh:mm tt")
                                : ""
                        });
                    }
                }
            }

            return list;
        }

        public bool SaveTrackerUser(GpsTrackerUserDto dto)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();

                string checkQuery = @"SELECT COUNT(1) FROM dbo.GpsTrackerUsers WHERE MobileNo = @MobileNo";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@MobileNo", dto.MobileNo ?? (object)DBNull.Value);

                    int exists = Convert.ToInt32(checkCmd.ExecuteScalar());
                    if (exists > 0)
                    {
                        return false;
                    }
                }

                string insertQuery = @"
                    INSERT INTO dbo.GpsTrackerUsers
                    (
                        UserId,
                        EmployeeCode,
                        EmployeeName,
                        MobileNo,
                        DepartmentName,
                        BranchName,
                        IsActive,
                        IsLiveTrackingEnabled,
                        IsFieldVisitEnabled,
                        CreatedAt
                    )
                    VALUES
                    (
                        @UserId,
                        @EmployeeCode,
                        @EmployeeName,
                        @MobileNo,
                        @DepartmentName,
                        @BranchName,
                        @IsActive,
                        @IsLiveTrackingEnabled,
                        @IsFieldVisitEnabled,
                        GETDATE()
                    )";

                using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", dto.UserId.HasValue ? (object)dto.UserId.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@EmployeeCode", string.IsNullOrWhiteSpace(dto.EmployeeCode) ? (object)DBNull.Value : dto.EmployeeCode);
                    cmd.Parameters.AddWithValue("@EmployeeName", string.IsNullOrWhiteSpace(dto.EmployeeName) ? (object)DBNull.Value : dto.EmployeeName);
                    cmd.Parameters.AddWithValue("@MobileNo", string.IsNullOrWhiteSpace(dto.MobileNo) ? (object)DBNull.Value : dto.MobileNo);
                    cmd.Parameters.AddWithValue("@DepartmentName", string.IsNullOrWhiteSpace(dto.DepartmentName) ? (object)DBNull.Value : dto.DepartmentName);
                    cmd.Parameters.AddWithValue("@BranchName", string.IsNullOrWhiteSpace(dto.BranchName) ? (object)DBNull.Value : dto.BranchName);
                    cmd.Parameters.AddWithValue("@IsActive", dto.IsActive);
                    cmd.Parameters.AddWithValue("@IsLiveTrackingEnabled", dto.IsLiveTrackingEnabled);
                    cmd.Parameters.AddWithValue("@IsFieldVisitEnabled", dto.IsFieldVisitEnabled);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public GpsTrackerUserDto GetTrackerUserByMobileNo(string mobileNo)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = @"
                    SELECT TOP 1
                        Id,
                        UserId,
                        EmployeeCode,
                        EmployeeName,
                        MobileNo,
                        DepartmentName,
                        BranchName,
                        IsActive,
                        IsLiveTrackingEnabled,
                        IsFieldVisitEnabled
                    FROM dbo.GpsTrackerUsers
                    WHERE MobileNo = @MobileNo
                      AND IsActive = 1
                      AND IsFieldVisitEnabled = 1";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MobileNo", mobileNo ?? (object)DBNull.Value);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new GpsTrackerUserDto
                            {
                                Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0,
                                UserId = reader["UserId"] != DBNull.Value ? (int?)Convert.ToInt32(reader["UserId"]) : null,
                                EmployeeCode = reader["EmployeeCode"] != DBNull.Value ? reader["EmployeeCode"].ToString() : "",
                                EmployeeName = reader["EmployeeName"] != DBNull.Value ? reader["EmployeeName"].ToString() : "",
                                MobileNo = reader["MobileNo"] != DBNull.Value ? reader["MobileNo"].ToString() : "",
                                DepartmentName = reader["DepartmentName"] != DBNull.Value ? reader["DepartmentName"].ToString() : "",
                                BranchName = reader["BranchName"] != DBNull.Value ? reader["BranchName"].ToString() : "",
                                IsActive = reader["IsActive"] != DBNull.Value && Convert.ToBoolean(reader["IsActive"]),
                                IsLiveTrackingEnabled = reader["IsLiveTrackingEnabled"] != DBNull.Value && Convert.ToBoolean(reader["IsLiveTrackingEnabled"]),
                                IsFieldVisitEnabled = reader["IsFieldVisitEnabled"] != DBNull.Value && Convert.ToBoolean(reader["IsFieldVisitEnabled"])
                            };
                        }
                    }
                }
            }

            return null;
        }

        public bool SaveFieldVisit(GpsFieldVisitSaveDto dto, string deviceInfo, string ipAddress)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = @"
                    INSERT INTO dbo.GpsFieldVisitLogs
                    (
                        TrackerUserId,
                        UserId,
                        EmployeeCode,
                        EmployeeName,
                        MobileNo,
                        VisitPurpose,
                        ClientName,
                        CollectionNo,
                        Remarks,
                        Latitude,
                        Longitude,
                        AccuracyMeter,
                        AddressText,
                        VisitDateTime,
                        DeviceInfo,
                        IpAddress
                    )
                    VALUES
                    (
                        @TrackerUserId,
                        @UserId,
                        @EmployeeCode,
                        @EmployeeName,
                        @MobileNo,
                        @VisitPurpose,
                        @ClientName,
                        @CollectionNo,
                        @Remarks,
                        @Latitude,
                        @Longitude,
                        @AccuracyMeter,
                        @AddressText,
                        GETDATE(),
                        @DeviceInfo,
                        @IpAddress
                    )";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TrackerUserId", dto.TrackerUserId);
                    cmd.Parameters.AddWithValue("@UserId", dto.UserId.HasValue ? (object)dto.UserId.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@EmployeeCode", string.IsNullOrWhiteSpace(dto.EmployeeCode) ? (object)DBNull.Value : dto.EmployeeCode);
                    cmd.Parameters.AddWithValue("@EmployeeName", string.IsNullOrWhiteSpace(dto.EmployeeName) ? (object)DBNull.Value : dto.EmployeeName);
                    cmd.Parameters.AddWithValue("@MobileNo", string.IsNullOrWhiteSpace(dto.MobileNo) ? (object)DBNull.Value : dto.MobileNo);
                    cmd.Parameters.AddWithValue("@VisitPurpose", string.IsNullOrWhiteSpace(dto.VisitPurpose) ? (object)DBNull.Value : dto.VisitPurpose);
                    cmd.Parameters.AddWithValue("@ClientName", string.IsNullOrWhiteSpace(dto.ClientName) ? (object)DBNull.Value : dto.ClientName);
                    cmd.Parameters.AddWithValue("@CollectionNo", string.IsNullOrWhiteSpace(dto.CollectionNo) ? (object)DBNull.Value : dto.CollectionNo);
                    cmd.Parameters.AddWithValue("@Remarks", string.IsNullOrWhiteSpace(dto.Remarks) ? (object)DBNull.Value : dto.Remarks);
                    cmd.Parameters.AddWithValue("@Latitude", dto.Latitude);
                    cmd.Parameters.AddWithValue("@Longitude", dto.Longitude);
                    cmd.Parameters.AddWithValue("@AccuracyMeter", dto.AccuracyMeter.HasValue ? (object)dto.AccuracyMeter.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@AddressText", string.IsNullOrWhiteSpace(dto.AddressText) ? (object)DBNull.Value : dto.AddressText);
                    cmd.Parameters.AddWithValue("@DeviceInfo", string.IsNullOrWhiteSpace(deviceInfo) ? (object)DBNull.Value : deviceInfo);
                    cmd.Parameters.AddWithValue("@IpAddress", string.IsNullOrWhiteSpace(ipAddress) ? (object)DBNull.Value : ipAddress);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
        public bool SaveLiveLocation(GpsLiveLocationSaveDto dto)
        {
            using (SqlConnection conn = new SqlConnection(GetAppConnectionString()))
            {
                string query = @"
            INSERT INTO dbo.GpsLiveTrackingLogs
            (
                TrackerUserId,
                MobileNo,
                EmployeeCode,
                EmployeeName,
                Latitude,
                Longitude,
                AccuracyMeter,
                DeviceInfo,
                IpAddress,
                EntryTime,
                IsActive
            )
            VALUES
            (
                @TrackerUserId,
                @MobileNo,
                @EmployeeCode,
                @EmployeeName,
                @Latitude,
                @Longitude,
                @AccuracyMeter,
                @DeviceInfo,
                @IpAddress,
                GETDATE(),
                1
            )";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TrackerUserId", dto.TrackerUserId);
                    cmd.Parameters.AddWithValue("@MobileNo", (object)dto.MobileNo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@EmployeeCode", (object)dto.EmployeeCode ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@EmployeeName", (object)dto.EmployeeName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Latitude", dto.Latitude);
                    cmd.Parameters.AddWithValue("@Longitude", dto.Longitude);
                    cmd.Parameters.AddWithValue("@AccuracyMeter", (object)dto.AccuracyMeter ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DeviceInfo", (object)dto.DeviceInfo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IpAddress", (object)dto.IpAddress ?? DBNull.Value);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
        public List<GpsLiveLocationDto> GetCurrentLiveLocations()
        {
            var result = new List<GpsLiveLocationDto>();

            using (SqlConnection conn = new SqlConnection(GetAppConnectionString()))
            {
                string query = @"
            ;WITH CTE AS
            (
                SELECT 
                    TrackerUserId,
                    MobileNo,
                    EmployeeCode,
                    EmployeeName,
                    Latitude,
                    Longitude,
                    AccuracyMeter,
                    EntryTime,
                    ROW_NUMBER() OVER (PARTITION BY MobileNo ORDER BY EntryTime DESC) AS RN
                FROM dbo.GpsLiveTrackingLogs
                WHERE IsActive = 1
                  AND EntryTime >= DATEADD(MINUTE, -5, GETDATE())
            )
            SELECT
                TrackerUserId,
                MobileNo,
                EmployeeCode,
                EmployeeName,
                Latitude,
                Longitude,
                AccuracyMeter,
                EntryTime
            FROM CTE
            WHERE RN = 1
            ORDER BY EntryTime DESC;";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new GpsLiveLocationDto
                            {
                                TrackerUserId = reader["TrackerUserId"] != DBNull.Value ? Convert.ToInt32(reader["TrackerUserId"]) : 0,
                                MobileNo = reader["MobileNo"]?.ToString(),
                                EmployeeCode = reader["EmployeeCode"]?.ToString(),
                                EmployeeName = reader["EmployeeName"]?.ToString(),
                                Latitude = Convert.ToDecimal(reader["Latitude"]),
                                Longitude = Convert.ToDecimal(reader["Longitude"]),
                                AccuracyMeter = reader["AccuracyMeter"] != DBNull.Value ? Convert.ToDecimal(reader["AccuracyMeter"]) : (decimal?)null,
                                EntryTime = Convert.ToDateTime(reader["EntryTime"])
                            });
                        }
                    }
                }
            }

            return result;
        }
        private string GetAppConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["AttandanceSyncConnection"].ConnectionString;
        }
        public List<GpsVisitHistoryDto> GetFieldVisitHistory()
        {
            var list = new List<GpsVisitHistoryDto>();

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = @"
            SELECT 
                Id,
                TrackerUserId,
                EmployeeCode,
                EmployeeName,
                MobileNo,
                VisitPurpose,
                ClientName,
                CollectionNo,
                Remarks,
                Latitude,
                Longitude,
                AccuracyMeter,
                AddressText,
                VisitDateTime
            FROM dbo.GpsFieldVisitLogs
            ORDER BY Id DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new GpsVisitHistoryDto();

                        item.Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0;
                        item.TrackerUserId = reader["TrackerUserId"] != DBNull.Value ? Convert.ToInt32(reader["TrackerUserId"]) : 0;
                        item.EmployeeCode = reader["EmployeeCode"] != DBNull.Value ? reader["EmployeeCode"].ToString() : "";
                        item.EmployeeName = reader["EmployeeName"] != DBNull.Value ? reader["EmployeeName"].ToString() : "";
                        item.MobileNo = reader["MobileNo"] != DBNull.Value ? reader["MobileNo"].ToString() : "";
                        item.VisitPurpose = reader["VisitPurpose"] != DBNull.Value ? reader["VisitPurpose"].ToString() : "";
                        item.ClientName = reader["ClientName"] != DBNull.Value ? reader["ClientName"].ToString() : "";
                        item.CollectionNo = reader["CollectionNo"] != DBNull.Value ? reader["CollectionNo"].ToString() : "";
                        item.Remarks = reader["Remarks"] != DBNull.Value ? reader["Remarks"].ToString() : "";
                        item.Latitude = reader["Latitude"] != DBNull.Value ? Convert.ToDecimal(reader["Latitude"]) : 0;
                        item.Longitude = reader["Longitude"] != DBNull.Value ? Convert.ToDecimal(reader["Longitude"]) : 0;
                        item.AccuracyMeter = reader["AccuracyMeter"] != DBNull.Value ? (decimal?)Convert.ToDecimal(reader["AccuracyMeter"]) : null;
                        item.AddressText = reader["AddressText"] != DBNull.Value ? reader["AddressText"].ToString() : "";
                        item.VisitDateTimeText = reader["VisitDateTime"] != DBNull.Value
                            ? Convert.ToDateTime(reader["VisitDateTime"]).ToString("dd-MMM-yyyy hh:mm tt")
                            : "";

                        list.Add(item);
                    }
                }
            }

            return list;
        }
    }
}