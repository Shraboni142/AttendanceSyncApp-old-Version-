using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using AttendanceSyncApp.Models.DTOs.EmployeeInformation;
using AttendanceSyncApp.Services.Interfaces;

namespace AttendanceSyncApp.Services
{
    public class AdminEmployeeInformationService : IAdminEmployeeInformationService
    {
        private string GetSmartToolsConnectionString()
        {
            return "Server=192.168.14.100;Database=Smart_Tools;User Id=intran;Password=!ntr@n321;Encrypt=False;TrustServerCertificate=True;";
        }

        public List<EmployeeInfoDropdownDto> GetEmployees()
        {
            var result = new List<EmployeeInfoDropdownDto>();

            using (SqlConnection conn = new SqlConnection(GetSmartToolsConnectionString()))
            {
                conn.Open();

                string query = @"
            SELECT
                Id,
                EmployeeCode,
                EmployeeName
            FROM dbo.EmployeeInfoEmployees
            WHERE IsActive = 1
            ORDER BY EmployeeCode";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new EmployeeInfoDropdownDto
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            EmployeeCode = reader["EmployeeCode"]?.ToString(),
                            EmployeeName = reader["EmployeeName"]?.ToString()
                        });
                    }
                }
            }

            return result;
        }

        public EmployeeInfoGeneralDto GetEmployeeGeneralInfo(string employeeCode)
        {
            EmployeeInfoGeneralDto data = null;

            using (SqlConnection conn = new SqlConnection(GetSmartToolsConnectionString()))
            {
                conn.Open();

                string query = @"
        SELECT TOP 1
            e.Id AS EmployeeMasterId,
            g.Id AS GeneralInfoId,
            e.EmployeeCode,
            e.EmployeeName,
            g.FatherName,
            g.MotherName,
            g.MobileNo,
            g.BasicSalary,
            g.DateOfBirth,
            g.DesignationId,
            g.DepartmentId,
            g.DesignationName,
            g.DepartmentName,
            g.BranchName
        FROM dbo.EmployeeInfoEmployees e
        LEFT JOIN dbo.EmployeeInfoGeneralInfos g
            ON LTRIM(RTRIM(e.EmployeeCode)) = LTRIM(RTRIM(g.EmployeeCode))
        WHERE LTRIM(RTRIM(e.EmployeeCode)) = LTRIM(RTRIM(@EmployeeCode))
          AND e.IsActive = 1";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeCode", employeeCode);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            data = new EmployeeInfoGeneralDto
                            {
                                Id = Convert.ToInt32(reader["EmployeeMasterId"]),
                                GeneralInfoId = reader["GeneralInfoId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["GeneralInfoId"]),
                                EmployeeCode = reader["EmployeeCode"]?.ToString(),
                                EmployeeName = reader["EmployeeName"]?.ToString(),
                                FatherName = reader["FatherName"]?.ToString(),
                                MotherName = reader["MotherName"]?.ToString(),
                                MobileNo = reader["MobileNo"]?.ToString(),
                                BasicSalary = reader["BasicSalary"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["BasicSalary"]),
                                DateOfBirth = reader["DateOfBirth"] == DBNull.Value ? "" : Convert.ToDateTime(reader["DateOfBirth"]).ToString("yyyy-MM-dd"),
                                DesignationId = reader["DesignationId"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["DesignationId"]),
                                DepartmentId = reader["DepartmentId"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["DepartmentId"]),
                                DesignationName = reader["DesignationName"]?.ToString(),
                                DepartmentName = reader["DepartmentName"]?.ToString(),
                                BranchName = reader["BranchName"]?.ToString()
                            };
                        }
                    }
                }
            }

            return data;
        }
        public bool UpdateEmployeeGeneralInfo(EmployeeInfoGeneralDto dto)
        {
            using (SqlConnection conn = new SqlConnection(GetSmartToolsConnectionString()))
            {
                conn.Open();

                string query = @"
DECLARE @EmployeeInfoEmployeeId INT;

SELECT TOP 1 @EmployeeInfoEmployeeId = Id
FROM dbo.EmployeeInfoEmployees
WHERE LTRIM(RTRIM(EmployeeCode)) = LTRIM(RTRIM(@EmployeeCode));

IF EXISTS (
    SELECT 1
    FROM dbo.EmployeeInfoGeneralInfos
    WHERE LTRIM(RTRIM(EmployeeCode)) = LTRIM(RTRIM(@EmployeeCode))
)
BEGIN
    UPDATE dbo.EmployeeInfoGeneralInfos
    SET
        EmployeeInfoEmployeeId = @EmployeeInfoEmployeeId,
        EmployeeName = @EmployeeName,
        FatherName = @FatherName,
        MotherName = @MotherName,
        MobileNo = @MobileNo,
        BasicSalary = @BasicSalary,
        DateOfBirth = @DateOfBirth,
        DesignationName = @DesignationName,
        DepartmentName = @DepartmentName,
        BranchName = @BranchName,
        UpdatedAt = GETDATE()
    WHERE LTRIM(RTRIM(EmployeeCode)) = LTRIM(RTRIM(@EmployeeCode))
END
ELSE
BEGIN
    INSERT INTO dbo.EmployeeInfoGeneralInfos
    (
        EmployeeInfoEmployeeId,
        EmployeeCode,
        EmployeeName,
        FatherName,
        MotherName,
        MobileNo,
        BasicSalary,
        DateOfBirth,
        DesignationName,
        DepartmentName,
        BranchName
    )
    VALUES
    (
        @EmployeeInfoEmployeeId,
        @EmployeeCode,
        @EmployeeName,
        @FatherName,
        @MotherName,
        @MobileNo,
        @BasicSalary,
        @DateOfBirth,
        @DesignationName,
        @DepartmentName,
        @BranchName
    )
END";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeCode", (object)dto.EmployeeCode ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@EmployeeName", (object)dto.EmployeeName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@FatherName", (object)dto.FatherName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MotherName", (object)dto.MotherName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MobileNo", (object)dto.MobileNo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@BasicSalary", (object)dto.BasicSalary ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DesignationName", (object)dto.DesignationName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DepartmentName", (object)dto.DepartmentName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@BranchName", (object)dto.BranchName ?? DBNull.Value);

                    if (string.IsNullOrWhiteSpace(dto.DateOfBirth))
                        cmd.Parameters.AddWithValue("@DateOfBirth", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("@DateOfBirth", Convert.ToDateTime(dto.DateOfBirth));

                    cmd.ExecuteNonQuery();
                }
            }

            return true;
        }
        public EmployeeInfoAddressDto GetEmployeeAddressInfo(string employeeCode)
        {
            EmployeeInfoAddressDto data = null;

            using (SqlConnection conn = new SqlConnection(GetSmartToolsConnectionString()))
            {
                conn.Open();

                int generalInfoId = GetGeneralInfoIdByEmployeeCode(employeeCode, conn);

                if (generalInfoId == 0)
                {
                    return new EmployeeInfoAddressDto();
                }

                string query = @"
        SELECT TOP 1
            PresentHouseVillageName,
            PresentHouseNo,
            PresentRoadNo,
            PresentBlock,
            PresentArea,
            PresentSector,
            PresentCountry,
            PresentDivision,
            PresentDistrict,
            PresentThanaUpazilla,
            PresentPostOffice,
            PermanentHouseVillageName,
            PermanentHouseNo,
            PermanentRoadNo,
            PermanentBlock,
            PermanentArea,
            PermanentSector,
            PermanentCountry,
            PermanentDivision,
            PermanentDistrict,
            PermanentThanaUpazilla,
            PermanentPostOffice
        FROM dbo.EmployeeInfoAddressInfos
        WHERE EmployeeInfoGeneralInfoId = @EmployeeInfoGeneralInfoId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeInfoGeneralInfoId", generalInfoId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            data = new EmployeeInfoAddressDto
                            {
                                PresentHouseVillageName = reader["PresentHouseVillageName"]?.ToString(),
                                PresentHouseNo = reader["PresentHouseNo"]?.ToString(),
                                PresentRoadNo = reader["PresentRoadNo"]?.ToString(),
                                PresentBlock = reader["PresentBlock"]?.ToString(),
                                PresentArea = reader["PresentArea"]?.ToString(),
                                PresentSector = reader["PresentSector"]?.ToString(),
                                PresentCountry = reader["PresentCountry"]?.ToString(),
                                PresentDivision = reader["PresentDivision"]?.ToString(),
                                PresentDistrict = reader["PresentDistrict"]?.ToString(),
                                PresentThanaUpazilla = reader["PresentThanaUpazilla"]?.ToString(),
                                PresentPostOffice = reader["PresentPostOffice"]?.ToString(),

                                PermanentHouseVillageName = reader["PermanentHouseVillageName"]?.ToString(),
                                PermanentHouseNo = reader["PermanentHouseNo"]?.ToString(),
                                PermanentRoadNo = reader["PermanentRoadNo"]?.ToString(),
                                PermanentBlock = reader["PermanentBlock"]?.ToString(),
                                PermanentArea = reader["PermanentArea"]?.ToString(),
                                PermanentSector = reader["PermanentSector"]?.ToString(),
                                PermanentCountry = reader["PermanentCountry"]?.ToString(),
                                PermanentDivision = reader["PermanentDivision"]?.ToString(),
                                PermanentDistrict = reader["PermanentDistrict"]?.ToString(),
                                PermanentThanaUpazilla = reader["PermanentThanaUpazilla"]?.ToString(),
                                PermanentPostOffice = reader["PermanentPostOffice"]?.ToString()
                            };
                        }
                    }
                }
            }

            return data ?? new EmployeeInfoAddressDto();
        }
        public bool UpdateEmployeeAddressInfo(string employeeCode, EmployeeInfoAddressDto dto)
        {
            using (SqlConnection conn = new SqlConnection(GetSmartToolsConnectionString()))
            {
                conn.Open();

                int generalInfoId = GetGeneralInfoIdByEmployeeCode(employeeCode, conn);

                if (generalInfoId == 0)
                {
                    throw new Exception("General information not found for this employee code.");
                }

                string query = @"
IF EXISTS (
    SELECT 1
    FROM dbo.EmployeeInfoAddressInfos
    WHERE EmployeeInfoGeneralInfoId = @EmployeeInfoGeneralInfoId
)
BEGIN
    UPDATE dbo.EmployeeInfoAddressInfos
    SET
        PresentHouseVillageName = @PresentHouseVillageName,
        PresentHouseNo = @PresentHouseNo,
        PresentRoadNo = @PresentRoadNo,
        PresentBlock = @PresentBlock,
        PresentArea = @PresentArea,
        PresentSector = @PresentSector,
        PresentCountry = @PresentCountry,
        PresentDivision = @PresentDivision,
        PresentDistrict = @PresentDistrict,
        PresentThanaUpazilla = @PresentThanaUpazilla,
        PresentPostOffice = @PresentPostOffice,
        PermanentHouseVillageName = @PermanentHouseVillageName,
        PermanentHouseNo = @PermanentHouseNo,
        PermanentRoadNo = @PermanentRoadNo,
        PermanentBlock = @PermanentBlock,
        PermanentArea = @PermanentArea,
        PermanentSector = @PermanentSector,
        PermanentCountry = @PermanentCountry,
        PermanentDivision = @PermanentDivision,
        PermanentDistrict = @PermanentDistrict,
        PermanentThanaUpazilla = @PermanentThanaUpazilla,
        PermanentPostOffice = @PermanentPostOffice,
        UpdatedAt = GETDATE()
    WHERE EmployeeInfoGeneralInfoId = @EmployeeInfoGeneralInfoId
END
ELSE
BEGIN
    INSERT INTO dbo.EmployeeInfoAddressInfos
    (
        EmployeeInfoGeneralInfoId,
        PresentHouseVillageName,
        PresentHouseNo,
        PresentRoadNo,
        PresentBlock,
        PresentArea,
        PresentSector,
        PresentCountry,
        PresentDivision,
        PresentDistrict,
        PresentThanaUpazilla,
        PresentPostOffice,
        PermanentHouseVillageName,
        PermanentHouseNo,
        PermanentRoadNo,
        PermanentBlock,
        PermanentArea,
        PermanentSector,
        PermanentCountry,
        PermanentDivision,
        PermanentDistrict,
        PermanentThanaUpazilla,
        PermanentPostOffice
    )
    VALUES
    (
        @EmployeeInfoGeneralInfoId,
        @PresentHouseVillageName,
        @PresentHouseNo,
        @PresentRoadNo,
        @PresentBlock,
        @PresentArea,
        @PresentSector,
        @PresentCountry,
        @PresentDivision,
        @PresentDistrict,
        @PresentThanaUpazilla,
        @PresentPostOffice,
        @PermanentHouseVillageName,
        @PermanentHouseNo,
        @PermanentRoadNo,
        @PermanentBlock,
        @PermanentArea,
        @PermanentSector,
        @PermanentCountry,
        @PermanentDivision,
        @PermanentDistrict,
        @PermanentThanaUpazilla,
        @PermanentPostOffice
    )
END";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeInfoGeneralInfoId", generalInfoId);

                    cmd.Parameters.AddWithValue("@PresentHouseVillageName", (object)dto.PresentHouseVillageName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PresentHouseNo", (object)dto.PresentHouseNo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PresentRoadNo", (object)dto.PresentRoadNo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PresentBlock", (object)dto.PresentBlock ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PresentArea", (object)dto.PresentArea ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PresentSector", (object)dto.PresentSector ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PresentCountry", (object)dto.PresentCountry ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PresentDivision", (object)dto.PresentDivision ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PresentDistrict", (object)dto.PresentDistrict ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PresentThanaUpazilla", (object)dto.PresentThanaUpazilla ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PresentPostOffice", (object)dto.PresentPostOffice ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@PermanentHouseVillageName", (object)dto.PermanentHouseVillageName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PermanentHouseNo", (object)dto.PermanentHouseNo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PermanentRoadNo", (object)dto.PermanentRoadNo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PermanentBlock", (object)dto.PermanentBlock ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PermanentArea", (object)dto.PermanentArea ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PermanentSector", (object)dto.PermanentSector ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PermanentCountry", (object)dto.PermanentCountry ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PermanentDivision", (object)dto.PermanentDivision ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PermanentDistrict", (object)dto.PermanentDistrict ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PermanentThanaUpazilla", (object)dto.PermanentThanaUpazilla ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PermanentPostOffice", (object)dto.PermanentPostOffice ?? DBNull.Value);

                    cmd.ExecuteNonQuery();
                }
            }

            return true;
        }
        public List<EmployeeEducationDto> GetEmployeeEducations(int employeeId)
        {
            var result = new List<EmployeeEducationDto>();

            using (SqlConnection conn = new SqlConnection(GetSmartToolsConnectionString()))
            {
                conn.Open();

                string employeeCode = GetEmployeeCodeById(employeeId, conn);

                string generalQuery = @"
        SELECT TOP 1 Id
        FROM dbo.EmployeeInfoGeneralInfos
        WHERE LTRIM(RTRIM(EmployeeCode)) = LTRIM(RTRIM(@EmployeeCode))";

                object generalObj;
                using (SqlCommand generalCmd = new SqlCommand(generalQuery, conn))
                {
                    generalCmd.Parameters.AddWithValue("@EmployeeCode", employeeCode ?? "");
                    generalObj = generalCmd.ExecuteScalar();
                }

                if (generalObj == null || generalObj == DBNull.Value)
                {
                    return result;
                }

                int generalInfoId = Convert.ToInt32(generalObj);

                string query = @"
        SELECT
            Id,
            EducationId,
            EducationName,
            [Group],
            Board,
            AcademicYear,
            AcademicInstitute,
            Division,
            Result
        FROM dbo.EmployeeInfoEducationInfos
        WHERE EmployeeInfoGeneralInfoId = @EmployeeInfoGeneralInfoId
        ORDER BY Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeInfoGeneralInfoId", generalInfoId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new EmployeeEducationDto
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                EducationId = reader["EducationId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["EducationId"]),
                                EducationName = reader["EducationName"]?.ToString(),
                                Group = reader["Group"]?.ToString(),
                                Board = reader["Board"]?.ToString(),
                                AcademicYear = reader["AcademicYear"]?.ToString(),
                                AcademicInstitute = reader["AcademicInstitute"]?.ToString(),
                                Division = reader["Division"]?.ToString(),
                                Result = reader["Result"]?.ToString()
                            });
                        }
                    }
                }
            }

            return result;
        }
        public List<DropdownItemDto> GetDesignations()
        {
            var result = new List<DropdownItemDto>();

            using (SqlConnection conn = new SqlConnection(GetSmartToolsConnectionString()))
            {
                conn.Open();

                string query = @"
            SELECT Id, DesignationName
            FROM dbo.EmployeeInfoDesignations
            WHERE IsActive = 1
              AND DesignationName IS NOT NULL
              AND LTRIM(RTRIM(DesignationName)) <> ''
            ORDER BY DesignationName";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new DropdownItemDto
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = reader["DesignationName"]?.ToString()
                        });
                    }
                }
            }

            return result;
        }

        public List<DropdownItemDto> GetDepartments()
        {
            var result = new List<DropdownItemDto>();

            using (SqlConnection conn = new SqlConnection(GetSmartToolsConnectionString()))
            {
                conn.Open();

                string query = @"
            SELECT Id, DepartmentName
            FROM dbo.EmployeeInfoDepartments
            WHERE IsActive = 1
              AND DepartmentName IS NOT NULL
              AND LTRIM(RTRIM(DepartmentName)) <> ''
            ORDER BY DepartmentName";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new DropdownItemDto
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = reader["DepartmentName"]?.ToString()
                        });
                    }
                }
            }

            return result;
        }
        public List<EducationDropdownDto> GetEducationDropdown()
        {
            var result = new List<EducationDropdownDto>();

            using (SqlConnection conn = new SqlConnection(GetSmartToolsConnectionString()))
            {
                conn.Open();

                string query = @"
            SELECT Id, EducationName
            FROM dbo.EmployeeInfoEducations
            WHERE IsActive = 1
            ORDER BY EducationName";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new EducationDropdownDto
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            EducationName = reader["EducationName"]?.ToString()
                        });
                    }
                }
            }

            return result;
        }
        public EducationFieldDropdownsDto GetEducationFieldDropdowns()
        {
            var result = new EducationFieldDropdownsDto
            {
                Groups = new List<string>(),
                Boards = new List<string>(),
                AcademicYears = new List<string>(),
                AcademicInstitutes = new List<string>(),
                Divisions = new List<string>(),
                Results = new List<string>()
            };

            using (SqlConnection conn = new SqlConnection(GetSmartToolsConnectionString()))
            {
                conn.Open();

                result.Groups = GetDistinctStringList(conn, @"
            SELECT FieldValue
            FROM dbo.EmployeeInfoEducationFieldValues
            WHERE IsActive = 1
              AND FieldType = 'Group'
              AND FieldValue IS NOT NULL
              AND LTRIM(RTRIM(FieldValue)) <> ''
            ORDER BY FieldValue");

                result.Boards = GetDistinctStringList(conn, @"
            SELECT FieldValue
            FROM dbo.EmployeeInfoEducationFieldValues
            WHERE IsActive = 1
              AND FieldType = 'Board'
              AND FieldValue IS NOT NULL
              AND LTRIM(RTRIM(FieldValue)) <> ''
            ORDER BY FieldValue");

                result.AcademicYears = GetDistinctStringList(conn, @"
            SELECT FieldValue
            FROM dbo.EmployeeInfoEducationFieldValues
            WHERE IsActive = 1
              AND FieldType = 'AcademicYear'
              AND FieldValue IS NOT NULL
              AND LTRIM(RTRIM(FieldValue)) <> ''
              AND FieldValue <> 'YYYY'
            ORDER BY FieldValue");

                result.AcademicInstitutes = GetDistinctStringList(conn, @"
            SELECT FieldValue
            FROM dbo.EmployeeInfoEducationFieldValues
            WHERE IsActive = 1
              AND FieldType = 'AcademicInstitute'
              AND FieldValue IS NOT NULL
              AND LTRIM(RTRIM(FieldValue)) <> ''
            ORDER BY FieldValue");

                result.Divisions = GetDistinctStringList(conn, @"
            SELECT FieldValue
            FROM dbo.EmployeeInfoEducationFieldValues
            WHERE IsActive = 1
              AND FieldType = 'Division'
              AND FieldValue IS NOT NULL
              AND LTRIM(RTRIM(FieldValue)) <> ''
            ORDER BY FieldValue");

                result.Results = GetDistinctStringList(conn, @"
            SELECT FieldValue
            FROM dbo.EmployeeInfoEducationFieldValues
            WHERE IsActive = 1
              AND FieldType = 'Result'
              AND FieldValue IS NOT NULL
              AND LTRIM(RTRIM(FieldValue)) <> ''
            ORDER BY FieldValue");
            }

            return result;
        }

        private List<string> GetDistinctStringList(SqlConnection conn, string query)
        {
            var list = new List<string>();

            using (SqlCommand cmd = new SqlCommand(query, conn))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (reader["FieldValue"] != DBNull.Value)
                    {
                        var value = reader["FieldValue"].ToString().Trim();

                        if (!string.IsNullOrWhiteSpace(value) && !list.Contains(value))
                        {
                            list.Add(value);
                        }
                    }
                }
            }

            return list;
        }
        public bool SaveEducation(string employeeCode, EmployeeEducationDto dto)
        {
            using (SqlConnection conn = new SqlConnection(GetSmartToolsConnectionString()))
            {
                conn.Open();

                int generalInfoId = GetGeneralInfoIdByEmployeeCode(employeeCode, conn);

                if (generalInfoId == 0)
                {
                    throw new Exception("General information not found for this employee code.");
                }

                string query = @"
DECLARE @EducationLookupId INT;

SELECT TOP 1 @EducationLookupId = Id
FROM dbo.EmployeeInfoEducations
WHERE
    (SourceEducationId = @EducationId)
    OR (LTRIM(RTRIM(EducationName)) = LTRIM(RTRIM(@EducationName)));

IF @EducationLookupId IS NULL
BEGIN
    SELECT TOP 1 @EducationLookupId = Id
    FROM dbo.EmployeeInfoEducations
    ORDER BY Id;
END

IF @EducationLookupId IS NULL
BEGIN
    RAISERROR('Education lookup not found in EmployeeInfoEducations.', 16, 1);
    RETURN;
END

IF @Id = 0
BEGIN
    INSERT INTO dbo.EmployeeInfoEducationInfos
    (
        EmployeeInfoGeneralInfoId,
        EducationLookupId,
        EducationId,
        EducationName,
        [Group],
        Board,
        AcademicYear,
        AcademicInstitute,
        Division,
        Result
    )
    VALUES
    (
        @EmployeeInfoGeneralInfoId,
        @EducationLookupId,
        @EducationId,
        @EducationName,
        @Group,
        @Board,
        @AcademicYear,
        @AcademicInstitute,
        @Division,
        @Result
    )
END
ELSE
BEGIN
    UPDATE dbo.EmployeeInfoEducationInfos
    SET
        EmployeeInfoGeneralInfoId = @EmployeeInfoGeneralInfoId,
        EducationLookupId = @EducationLookupId,
        EducationId = @EducationId,
        EducationName = @EducationName,
        [Group] = @Group,
        Board = @Board,
        AcademicYear = @AcademicYear,
        AcademicInstitute = @AcademicInstitute,
        Division = @Division,
        Result = @Result,
        UpdatedAt = GETDATE()
    WHERE Id = @Id
END";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeInfoGeneralInfoId", generalInfoId);
                    cmd.Parameters.AddWithValue("@Id", dto.Id);
                    cmd.Parameters.AddWithValue("@EducationId", dto.EducationId);
                    cmd.Parameters.AddWithValue("@EducationName", (object)dto.EducationName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Group", (object)dto.Group ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Board", (object)dto.Board ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AcademicYear", (object)dto.AcademicYear ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AcademicInstitute", (object)dto.AcademicInstitute ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Division", (object)dto.Division ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Result", (object)dto.Result ?? DBNull.Value);

                    cmd.ExecuteNonQuery();
                }
            }

            return true;
        }

        public bool DeleteEducation(int id)
        {
            using (SqlConnection conn = new SqlConnection(GetSmartToolsConnectionString()))
            {
                conn.Open();

                string query = "DELETE FROM dbo.EmployeeInfoEducationInfos WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }

            return true;
        }
        public bool SaveAllEmployeeInformation(EmployeeFullInformationSaveDto dto)
        {
            using (SqlConnection conn = new SqlConnection(GetSmartToolsConnectionString()))
            {
                conn.Open();

                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        int employeeInfoEmployeeId = EnsureEmployeeMasterWithTransaction(dto.GeneralInfo, conn, transaction);

                        int generalInfoId = SaveGeneralInfoWithTransaction(dto.GeneralInfo, employeeInfoEmployeeId, conn, transaction);


                        // dto.AddressInfo.EmployeeId=generalInfoId;
                        SaveAddressInfoWithTransaction(generalInfoId, dto.GeneralInfo.EmployeeCode, dto.AddressInfo, conn, transaction);

                        //foreach (var item in dto.Educations)
                        //{
                        //    item.EmployeeId= generalInfoId;
                        //}
                        SyncEducationInfosWithTransaction(generalInfoId, dto.GeneralInfo.EmployeeCode, dto.Educations, conn, transaction);

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        public List<EmployeeProfileListDto> GetEmployeeProfiles()
        {
            var result = new List<EmployeeProfileListDto>();

            using (SqlConnection conn = new SqlConnection(GetSmartToolsConnectionString()))
            {
                conn.Open();

                string query = @"
        SELECT
            g.Id AS GeneralInfoId,
            g.EmployeeCode,
            g.EmployeeName,
            ISNULL(a.PresentHouseVillageName, '') AS HomeAddress
        FROM dbo.EmployeeInfoGeneralInfos g
        LEFT JOIN dbo.EmployeeInfoAddressInfos a
            ON a.EmployeeInfoGeneralInfoId = g.Id
        ORDER BY g.Id DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new EmployeeProfileListDto
                        {
                            GeneralInfoId = Convert.ToInt32(reader["GeneralInfoId"]),
                            EmployeeCode = reader["EmployeeCode"]?.ToString(),
                            EmployeeName = reader["EmployeeName"]?.ToString(),
                            HomeAddress = reader["HomeAddress"]?.ToString()
                        });
                    }
                }
            }

            return result;
        }

        private int EnsureEmployeeMasterWithTransaction(EmployeeInfoGeneralDto dto, SqlConnection conn, SqlTransaction transaction)
        {
            string query = @"
DECLARE @EmployeeInfoEmployeeId INT;

SELECT TOP 1 @EmployeeInfoEmployeeId = Id
FROM dbo.EmployeeInfoEmployees
WHERE LTRIM(RTRIM(EmployeeCode)) = LTRIM(RTRIM(@EmployeeCode));

IF @EmployeeInfoEmployeeId IS NOT NULL
BEGIN
    UPDATE dbo.EmployeeInfoEmployees
    SET
        EmployeeName = @EmployeeName,
        IsActive = 1
    WHERE Id = @EmployeeInfoEmployeeId;

    SELECT @EmployeeInfoEmployeeId;
END
ELSE
BEGIN
    INSERT INTO dbo.EmployeeInfoEmployees
    (
        SourceEmployeeId,
        EmployeeCode,
        EmployeeName,
        IsActive,
        CreatedAt
    )
    VALUES
    (
        NULL,
        @EmployeeCode,
        @EmployeeName,
        1,
        GETDATE()
    );

    SELECT CAST(SCOPE_IDENTITY() AS INT);
END";

            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@EmployeeCode", (object)dto.EmployeeCode ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@EmployeeName", (object)dto.EmployeeName ?? DBNull.Value);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public EmployeeFullInformationSaveDto GetEmployeeFullInformationByCode(string employeeCode)
        {
            var result = new EmployeeFullInformationSaveDto
            {
                GeneralInfo = new EmployeeInfoGeneralDto(),
                AddressInfo = new EmployeeInfoAddressDto(),
                Educations = new List<EmployeeEducationDto>()
            };

            using (SqlConnection conn = new SqlConnection(GetSmartToolsConnectionString()))
            {
                conn.Open();

                result.GeneralInfo = GetEmployeeGeneralInfo(employeeCode);
                result.AddressInfo = GetEmployeeAddressInfo(employeeCode);

                int generalInfoId = GetGeneralInfoIdByEmployeeCode(employeeCode, conn);

                if (generalInfoId > 0)
                {
                    string query = @"
            SELECT
                Id,
                EducationId,
                EducationName,
                [Group],
                Board,
                AcademicYear,
                AcademicInstitute,
                Division,
                Result
            FROM dbo.EmployeeInfoEducationInfos
            WHERE EmployeeInfoGeneralInfoId = @EmployeeInfoGeneralInfoId
            ORDER BY Id";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeInfoGeneralInfoId", generalInfoId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Educations.Add(new EmployeeEducationDto
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    EducationId = reader["EducationId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["EducationId"]),
                                    EducationName = reader["EducationName"]?.ToString(),
                                    Group = reader["Group"]?.ToString(),
                                    Board = reader["Board"]?.ToString(),
                                    AcademicYear = reader["AcademicYear"]?.ToString(),
                                    AcademicInstitute = reader["AcademicInstitute"]?.ToString(),
                                    Division = reader["Division"]?.ToString(),
                                    Result = reader["Result"]?.ToString()
                                });
                            }
                        }
                    }
                }
            }

            return result;
        }
        private int SaveGeneralInfoWithTransaction(EmployeeInfoGeneralDto dto, int employeeInfoEmployeeId, SqlConnection conn, SqlTransaction transaction)
        {
            string query = @"
DECLARE @ExistingGeneralInfoId INT;

SELECT TOP 1 @ExistingGeneralInfoId = Id
FROM dbo.EmployeeInfoGeneralInfos
WHERE LTRIM(RTRIM(EmployeeCode)) = LTRIM(RTRIM(@EmployeeCode));

IF @ExistingGeneralInfoId IS NOT NULL
BEGIN
    UPDATE dbo.EmployeeInfoGeneralInfos
    SET
        EmployeeInfoEmployeeId = @EmployeeInfoEmployeeId,
        EmployeeCode = @EmployeeCode,
        EmployeeName = @EmployeeName,
        FatherName = @FatherName,
        MotherName = @MotherName,
        MobileNo = @MobileNo,
        BasicSalary = @BasicSalary,
        DateOfBirth = @DateOfBirth,
        DesignationId = @DesignationId,
        DepartmentId = @DepartmentId,
        DesignationName = @DesignationName,
        DepartmentName = @DepartmentName,
        BranchName = @BranchName,
        UpdatedAt = GETDATE()
    WHERE Id = @ExistingGeneralInfoId;

    SELECT @ExistingGeneralInfoId;
END
ELSE
BEGIN
    INSERT INTO dbo.EmployeeInfoGeneralInfos
    (
        EmployeeInfoEmployeeId,
        EmployeeCode,
        EmployeeName,
        FatherName,
        MotherName,
        MobileNo,
        BasicSalary,
        DateOfBirth,
        DesignationId,
        DepartmentId,
        DesignationName,
        DepartmentName,
        BranchName
    )
    VALUES
    (
        @EmployeeInfoEmployeeId,
        @EmployeeCode,
        @EmployeeName,
        @FatherName,
        @MotherName,
        @MobileNo,
        @BasicSalary,
        @DateOfBirth,
        @DesignationId,
        @DepartmentId,
        @DesignationName,
        @DepartmentName,
        @BranchName
    );

    SELECT CAST(SCOPE_IDENTITY() AS INT);
END";

            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@EmployeeInfoEmployeeId", employeeInfoEmployeeId);
                cmd.Parameters.AddWithValue("@EmployeeCode", (object)dto.EmployeeCode ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@EmployeeName", (object)dto.EmployeeName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FatherName", (object)dto.FatherName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MotherName", (object)dto.MotherName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MobileNo", (object)dto.MobileNo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@BasicSalary", (object)dto.BasicSalary ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DesignationId", (object)dto.DesignationId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DepartmentId", (object)dto.DepartmentId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DesignationName", (object)dto.DesignationName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DepartmentName", (object)dto.DepartmentName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@BranchName", (object)dto.BranchName ?? DBNull.Value);

                if (string.IsNullOrWhiteSpace(dto.DateOfBirth))
                    cmd.Parameters.AddWithValue("@DateOfBirth", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("@DateOfBirth", Convert.ToDateTime(dto.DateOfBirth));

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
        private void SaveAddressInfoWithTransaction(int generalInfoId, string employeeCode, EmployeeInfoAddressDto dto, SqlConnection conn, SqlTransaction transaction)
        {
            string query = @"
IF EXISTS (
    SELECT 1
    FROM dbo.EmployeeInfoAddressInfos
    WHERE EmployeeInfoGeneralInfoId = @EmployeeInfoGeneralInfoId
)
BEGIN
    UPDATE dbo.EmployeeInfoAddressInfos
    SET
        PresentHouseVillageName = @PresentHouseVillageName,
        PresentHouseNo = @PresentHouseNo,
        PresentRoadNo = @PresentRoadNo,
        PresentBlock = @PresentBlock,
        PresentArea = @PresentArea,
        PresentSector = @PresentSector,
        PresentCountry = @PresentCountry,
        PresentDivision = @PresentDivision,
        PresentDistrict = @PresentDistrict,
        PresentThanaUpazilla = @PresentThanaUpazilla,
        PresentPostOffice = @PresentPostOffice,
        PermanentHouseVillageName = @PermanentHouseVillageName,
        PermanentHouseNo = @PermanentHouseNo,
        PermanentRoadNo = @PermanentRoadNo,
        PermanentBlock = @PermanentBlock,
        PermanentArea = @PermanentArea,
        PermanentSector = @PermanentSector,
        PermanentCountry = @PermanentCountry,
        PermanentDivision = @PermanentDivision,
        PermanentDistrict = @PermanentDistrict,
        PermanentThanaUpazilla = @PermanentThanaUpazilla,
        PermanentPostOffice = @PermanentPostOffice,
        UpdatedAt = GETDATE()
    WHERE EmployeeInfoGeneralInfoId = @EmployeeInfoGeneralInfoId
END
ELSE
BEGIN
    INSERT INTO dbo.EmployeeInfoAddressInfos
    (
        EmployeeInfoGeneralInfoId,
        PresentHouseVillageName,
        PresentHouseNo,
        PresentRoadNo,
        PresentBlock,
        PresentArea,
        PresentSector,
        PresentCountry,
        PresentDivision,
        PresentDistrict,
        PresentThanaUpazilla,
        PresentPostOffice,
        PermanentHouseVillageName,
        PermanentHouseNo,
        PermanentRoadNo,
        PermanentBlock,
        PermanentArea,
        PermanentSector,
        PermanentCountry,
        PermanentDivision,
        PermanentDistrict,
        PermanentThanaUpazilla,
        PermanentPostOffice
    )
    VALUES
    (
        @EmployeeInfoGeneralInfoId,
        @PresentHouseVillageName,
        @PresentHouseNo,
        @PresentRoadNo,
        @PresentBlock,
        @PresentArea,
        @PresentSector,
        @PresentCountry,
        @PresentDivision,
        @PresentDistrict,
        @PresentThanaUpazilla,
        @PresentPostOffice,
        @PermanentHouseVillageName,
        @PermanentHouseNo,
        @PermanentRoadNo,
        @PermanentBlock,
        @PermanentArea,
        @PermanentSector,
        @PermanentCountry,
        @PermanentDivision,
        @PermanentDistrict,
        @PermanentThanaUpazilla,
        @PermanentPostOffice
    )
END";

            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@EmployeeInfoGeneralInfoId", generalInfoId);

                cmd.Parameters.AddWithValue("@PresentHouseVillageName", (object)dto.PresentHouseVillageName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PresentHouseNo", (object)dto.PresentHouseNo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PresentRoadNo", (object)dto.PresentRoadNo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PresentBlock", (object)dto.PresentBlock ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PresentArea", (object)dto.PresentArea ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PresentSector", (object)dto.PresentSector ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PresentCountry", (object)dto.PresentCountry ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PresentDivision", (object)dto.PresentDivision ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PresentDistrict", (object)dto.PresentDistrict ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PresentThanaUpazilla", (object)dto.PresentThanaUpazilla ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PresentPostOffice", (object)dto.PresentPostOffice ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@PermanentHouseVillageName", (object)dto.PermanentHouseVillageName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PermanentHouseNo", (object)dto.PermanentHouseNo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PermanentRoadNo", (object)dto.PermanentRoadNo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PermanentBlock", (object)dto.PermanentBlock ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PermanentArea", (object)dto.PermanentArea ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PermanentSector", (object)dto.PermanentSector ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PermanentCountry", (object)dto.PermanentCountry ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PermanentDivision", (object)dto.PermanentDivision ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PermanentDistrict", (object)dto.PermanentDistrict ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PermanentThanaUpazilla", (object)dto.PermanentThanaUpazilla ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PermanentPostOffice", (object)dto.PermanentPostOffice ?? DBNull.Value);

                cmd.ExecuteNonQuery();
            }
        }

        private void SyncEducationInfosWithTransaction(int generalInfoId, string employeeCode, List<EmployeeEducationDto> educations, SqlConnection conn, SqlTransaction transaction)
        {
            if (educations == null)
            {
                educations = new List<EmployeeEducationDto>();
            }

            string deleteMissingQuery = @"
    DELETE FROM dbo.EmployeeInfoEducationInfos
    WHERE EmployeeInfoGeneralInfoId = @EmployeeInfoGeneralInfoId
      AND Id NOT IN (
            SELECT Id
            FROM dbo.EmployeeInfoEducationInfos
            WHERE 1 = 0
      )";

            var incomingExistingIds = new List<int>();
            foreach (var item in educations)
            {
                if (item.Id > 0)
                {
                    incomingExistingIds.Add(item.Id);
                }
            }

            string deleteQuery;
            if (incomingExistingIds.Count == 0)
            {
                deleteQuery = @"
        DELETE FROM dbo.EmployeeInfoEducationInfos
        WHERE EmployeeInfoGeneralInfoId = @EmployeeInfoGeneralInfoId";
            }
            else
            {
                deleteQuery = $@"
        DELETE FROM dbo.EmployeeInfoEducationInfos
        WHERE EmployeeInfoGeneralInfoId = @EmployeeInfoGeneralInfoId
          AND Id NOT IN ({string.Join(",", incomingExistingIds)})";
            }

            using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn, transaction))
            {
                deleteCmd.Parameters.AddWithValue("@EmployeeInfoGeneralInfoId", generalInfoId);
                deleteCmd.ExecuteNonQuery();
            }

            foreach (var dto in educations)
            {
                SaveOrUpdateEducationRow(generalInfoId, employeeCode, dto, conn, transaction);
            }
        }

        private void SaveOrUpdateEducationRow(int generalInfoId, string employeeCode, EmployeeEducationDto dto, SqlConnection conn, SqlTransaction transaction)
        {
            string query = @"
DECLARE @EducationLookupId INT;

SELECT TOP 1 @EducationLookupId = Id
FROM dbo.EmployeeInfoEducations
WHERE
    (SourceEducationId = @EducationId)
    OR (LTRIM(RTRIM(EducationName)) = LTRIM(RTRIM(@EducationName)));

IF @EducationLookupId IS NULL
BEGIN
    SELECT TOP 1 @EducationLookupId = Id
    FROM dbo.EmployeeInfoEducations
    ORDER BY Id;
END

IF @Id = 0
BEGIN
    INSERT INTO dbo.EmployeeInfoEducationInfos
    (
        EmployeeInfoGeneralInfoId,
        EducationLookupId,
        EducationId,
        EducationName,
        [Group],
        Board,
        AcademicYear,
        AcademicInstitute,
        Division,
        Result
    )
    VALUES
    (
        @EmployeeInfoGeneralInfoId,
        @EducationLookupId,
        @EducationId,
        @EducationName,
        @Group,
        @Board,
        @AcademicYear,
        @AcademicInstitute,
        @Division,
        @Result
    )
END
ELSE
BEGIN
    UPDATE dbo.EmployeeInfoEducationInfos
    SET
        EmployeeInfoGeneralInfoId = @EmployeeInfoGeneralInfoId,
        EducationLookupId = @EducationLookupId,
        EducationId = @EducationId,
        EducationName = @EducationName,
        [Group] = @Group,
        Board = @Board,
        AcademicYear = @AcademicYear,
        AcademicInstitute = @AcademicInstitute,
        Division = @Division,
        Result = @Result,
        UpdatedAt = GETDATE()
    WHERE Id = @Id
END";

            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@EmployeeInfoGeneralInfoId", generalInfoId);
                cmd.Parameters.AddWithValue("@Id", dto.Id);
                cmd.Parameters.AddWithValue("@EducationId", dto.EducationId);
                cmd.Parameters.AddWithValue("@EducationName", (object)dto.EducationName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Group", (object)dto.Group ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Board", (object)dto.Board ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@AcademicYear", (object)dto.AcademicYear ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@AcademicInstitute", (object)dto.AcademicInstitute ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Division", (object)dto.Division ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Result", (object)dto.Result ?? DBNull.Value);

                cmd.ExecuteNonQuery();
            }
        }
        private void SaveEducationWithTransaction(string employeeCode, EmployeeEducationDto dto, SqlConnection conn, SqlTransaction transaction)
        {
            int generalInfoId = GetGeneralInfoIdByEmployeeCode(employeeCode, conn, transaction);

            if (generalInfoId == 0)
            {
                throw new Exception("General information not found for this employee code.");
            }

            SaveOrUpdateEducationRow(generalInfoId, employeeCode, dto, conn, transaction);
        }
        private int GetGeneralInfoIdByEmployeeCode(string employeeCode, SqlConnection conn, SqlTransaction transaction = null)
        {
            string query = @"
    SELECT TOP 1 Id
    FROM dbo.EmployeeInfoGeneralInfos
    WHERE LTRIM(RTRIM(EmployeeCode)) = LTRIM(RTRIM(@EmployeeCode))";

            using (SqlCommand cmd = transaction == null
                ? new SqlCommand(query, conn)
                : new SqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@EmployeeCode", employeeCode ?? "");

                object result = cmd.ExecuteScalar();
                if (result == null || result == DBNull.Value)
                    return 0;

                return Convert.ToInt32(result);
            }
        }
        private string GetEmployeeCodeById(int employeeId, SqlConnection conn)
        {
            string code = null;

            string query = @"
        SELECT EmployeeCode
        FROM dbo.EmployeeInfoEmployees
        WHERE Id = @Id";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Id", employeeId);

                object result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    code = result.ToString();
                }
            }

            return code;
        }
    }
}