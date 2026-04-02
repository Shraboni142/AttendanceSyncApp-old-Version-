using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using AttendanceSyncApp.Models.DTOs.EmployeeInformation;
using AttendanceSyncApp.Services.Interfaces;

namespace AttendanceSyncApp.Services
{
    public class AdminEmployeeInformationService : IAdminEmployeeInformationService
    {
        private string GetConnectionString()
        {
            return "Server=192.168.14.100;Database=Smart_v4_seba_Test;User Id=intran;Password=!ntr@n321;Encrypt=False;TrustServerCertificate=True;";
        }

        public List<EmployeeInfoDropdownDto> GetEmployees()
        {
            var result = new List<EmployeeInfoDropdownDto>();

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = @"
                    SELECT 
                        e.Id,
                        e.EmployeeId AS EmployeeCode,
                        LTRIM(RTRIM(
                            ISNULL(e.FirstName,'') + ' ' +
                            ISNULL(e.MiddleName,'') + ' ' +
                            ISNULL(e.LastName,'')
                        )) AS EmployeeName
                    FROM dbo.Employees e
                    ORDER BY e.EmployeeId";

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

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = @"
                    SELECT
                        e.Id,
                        e.EmployeeId AS EmployeeCode,
                        LTRIM(RTRIM(
                            ISNULL(e.FirstName,'') + ' ' +
                            ISNULL(e.MiddleName,'') + ' ' +
                            ISNULL(e.LastName,'')
                        )) AS EmployeeName,
                        e.FatherName,
                        e.MotherName,
                        e.MobileNo,
                        e.BasicSalary,
                        e.DateOfBirth,
                        d.DesignationName,
                        dp.DepartmentName,
                        b.BranchName
                    FROM dbo.Employees e
                    LEFT JOIN dbo.Designations d ON e.DesignationId = d.Id
                    LEFT JOIN dbo.Departments dp ON e.DepartmentId = dp.Id
                    LEFT JOIN dbo.Branches b ON e.BranchId = b.Id
                    WHERE e.EmployeeId = @EmployeeCode";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeCode", employeeCode);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            data = new EmployeeInfoGeneralDto
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                EmployeeCode = reader["EmployeeCode"]?.ToString(),
                                EmployeeName = reader["EmployeeName"]?.ToString(),
                                FatherName = reader["FatherName"]?.ToString(),
                                MotherName = reader["MotherName"]?.ToString(),
                                MobileNo = reader["MobileNo"]?.ToString(),
                                BasicSalary = reader["BasicSalary"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["BasicSalary"]),
                                DateOfBirth = reader["DateOfBirth"] == DBNull.Value ? "" : Convert.ToDateTime(reader["DateOfBirth"]).ToString("yyyy-MM-dd"),
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
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = @"
                    UPDATE dbo.Employees
                    SET
                        FatherName = @FatherName,
                        MotherName = @MotherName,
                        MobileNo = @MobileNo,
                        BasicSalary = @BasicSalary,
                        DateOfBirth = @DateOfBirth
                    WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FatherName", (object)dto.FatherName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MotherName", (object)dto.MotherName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MobileNo", (object)dto.MobileNo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@BasicSalary", (object)dto.BasicSalary ?? DBNull.Value);

                    if (string.IsNullOrWhiteSpace(dto.DateOfBirth))
                        cmd.Parameters.AddWithValue("@DateOfBirth", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("@DateOfBirth", Convert.ToDateTime(dto.DateOfBirth));

                    cmd.Parameters.AddWithValue("@Id", dto.Id);

                    cmd.ExecuteNonQuery();
                }
            }

            return true;
        }

        public EmployeeInfoAddressDto GetEmployeeAddressInfo(string employeeCode)
        {
            EmployeeInfoAddressDto data = null;

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = @"
                    SELECT
                        HouseName,
                        HouseNo,
                        RoadNo,
                        Block,
                        Area,
                        Sector,
                        CountryId,
                        DivisionId,
                        DistrictId,
                        ThanaId,
                        PostOfficeId,

                        PerHouseName,
                        PerHouseNo,
                        PerRoadNo,
                        PerBlock,
                        PerArea,
                        PerSector,
                        PerCountryId,
                        PerDivisionId,
                        PerDistrictId,
                        PerThanaId,
                        PerPostOfficeId
                    FROM dbo.Employees
                    WHERE EmployeeId = @EmployeeCode";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeCode", employeeCode);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            data = new EmployeeInfoAddressDto
                            {
                                PresentHouseVillageName = reader["HouseName"]?.ToString(),
                                PresentHouseNo = reader["HouseNo"]?.ToString(),
                                PresentRoadNo = reader["RoadNo"]?.ToString(),
                                PresentBlock = reader["Block"]?.ToString(),
                                PresentArea = reader["Area"]?.ToString(),
                                PresentSector = reader["Sector"]?.ToString(),
                                PresentCountry = reader["CountryId"]?.ToString(),
                                PresentDivision = reader["DivisionId"]?.ToString(),
                                PresentDistrict = reader["DistrictId"]?.ToString(),
                                PresentThanaUpazilla = reader["ThanaId"]?.ToString(),
                                PresentPostOffice = reader["PostOfficeId"]?.ToString(),

                                PermanentHouseVillageName = reader["PerHouseName"]?.ToString(),
                                PermanentHouseNo = reader["PerHouseNo"]?.ToString(),
                                PermanentRoadNo = reader["PerRoadNo"]?.ToString(),
                                PermanentBlock = reader["PerBlock"]?.ToString(),
                                PermanentArea = reader["PerArea"]?.ToString(),
                                PermanentSector = reader["PerSector"]?.ToString(),
                                PermanentCountry = reader["PerCountryId"]?.ToString(),
                                PermanentDivision = reader["PerDivisionId"]?.ToString(),
                                PermanentDistrict = reader["PerDistrictId"]?.ToString(),
                                PermanentThanaUpazilla = reader["PerThanaId"]?.ToString(),
                                PermanentPostOffice = reader["PerPostOfficeId"]?.ToString()
                            };
                        }
                    }
                }
            }

            return data;
        }

        public bool UpdateEmployeeAddressInfo(string employeeCode, EmployeeInfoAddressDto dto)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = @"
            UPDATE dbo.Employees
            SET
                HouseName = @PresentHouseVillageName,
                HouseNo = @PresentHouseNo,
                RoadNo = @PresentRoadNo,
                Block = @PresentBlock,
                Area = @PresentArea,
                Sector = @PresentSector,
                CountryId = @PresentCountry,
                DivisionId = @PresentDivision,
                DistrictId = @PresentDistrict,
                ThanaId = @PresentThanaUpazilla,
                PostOfficeId = @PresentPostOffice,

                PerHouseName = @PermanentHouseVillageName,
                PerHouseNo = @PermanentHouseNo,
                PerRoadNo = @PermanentRoadNo,
                PerBlock = @PermanentBlock,
                PerArea = @PermanentArea,
                PerSector = @PermanentSector,
                PerCountryId = @PermanentCountry,
                PerDivisionId = @PermanentDivision,
                PerDistrictId = @PermanentDistrict,
                PerThanaId = @PermanentThanaUpazilla,
                PerPostOfficeId = @PermanentPostOffice
            WHERE LTRIM(RTRIM(CAST(EmployeeId AS NVARCHAR(50)))) = LTRIM(RTRIM(@EmployeeCode))";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
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

                    cmd.Parameters.AddWithValue("@EmployeeCode", employeeCode);

                    cmd.ExecuteNonQuery();

                   
                }
            }

            return true;
        }

        public List<EmployeeEducationDto> GetEmployeeEducations(int employeeId)
        {
            var result = new List<EmployeeEducationDto>();

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = @"
    SELECT
        eu.Id,
        eu.EmployeeId,
        eu.EducationId,
        e.EducationName,
        eu.[Group],
        eu.Board,
        eu.AcademicYear,
        eu.AcademicInstitute,
        eu.Division,
        CAST(eu.CGPA AS NVARCHAR(100)) AS Result
    FROM dbo.EducationUnits eu
    LEFT JOIN dbo.Educations e ON eu.EducationId = e.Id
    WHERE eu.EmployeeId = @EmployeeId
    ORDER BY eu.Id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeId", employeeId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new EmployeeEducationDto
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                EmployeeId = Convert.ToInt32(reader["EmployeeId"]),
                                EducationId = Convert.ToInt32(reader["EducationId"]),
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

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = @"
            SELECT Id, DesignationName
            FROM dbo.Designations
            WHERE DesignationName IS NOT NULL
              AND LTRIM(RTRIM(DesignationName)) <> ''
            ORDER BY DesignationName";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new DropdownItemDto
                        {
                            Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0,
                            Name = reader["DesignationName"] != DBNull.Value ? reader["DesignationName"].ToString() : ""
                        });
                    }
                }
            }

            return result;
        }

        public List<DropdownItemDto> GetDepartments()
        {
            var result = new List<DropdownItemDto>();

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = @"
            SELECT Id, DepartmentName
            FROM dbo.Departments
            WHERE DepartmentName IS NOT NULL
              AND LTRIM(RTRIM(DepartmentName)) <> ''
            ORDER BY DepartmentName";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new DropdownItemDto
                        {
                            Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0,
                            Name = reader["DepartmentName"] != DBNull.Value ? reader["DepartmentName"].ToString() : ""
                        });
                    }
                }
            }

            return result;
        }
        public List<EducationDropdownDto> GetEducationDropdown()
        {
            var result = new List<EducationDropdownDto>();

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = @"SELECT Id, EducationName FROM dbo.Educations WHERE IsDeleted = 0 ORDER BY EducationName";

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

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();

                result.Groups = GetDistinctStringList(conn, @"
            SELECT DISTINCT CAST([Group] AS NVARCHAR(500)) AS Value
            FROM dbo.EducationUnits
            WHERE [Group] IS NOT NULL
              AND LTRIM(RTRIM(CAST([Group] AS NVARCHAR(500)))) <> ''
            ORDER BY Value");

                result.Boards = GetDistinctStringList(conn, @"
            SELECT DISTINCT CAST([Board] AS NVARCHAR(500)) AS Value
            FROM dbo.EducationUnits
            WHERE [Board] IS NOT NULL
              AND LTRIM(RTRIM(CAST([Board] AS NVARCHAR(500)))) <> ''
            ORDER BY Value");

                result.AcademicYears = GetDistinctStringList(conn, @"
            SELECT DISTINCT CAST([AcademicYear] AS NVARCHAR(500)) AS Value
            FROM dbo.EducationUnits
            WHERE [AcademicYear] IS NOT NULL
              AND LTRIM(RTRIM(CAST([AcademicYear] AS NVARCHAR(500)))) <> ''
              AND CAST([AcademicYear] AS NVARCHAR(500)) <> 'YYYY'
            ORDER BY Value");

                result.AcademicInstitutes = GetDistinctStringList(conn, @"
            SELECT DISTINCT CAST([AcademicInstitute] AS NVARCHAR(1000)) AS Value
            FROM dbo.EducationUnits
            WHERE [AcademicInstitute] IS NOT NULL
              AND LTRIM(RTRIM(CAST([AcademicInstitute] AS NVARCHAR(1000)))) <> ''
            ORDER BY Value");

                result.Divisions = GetDistinctStringList(conn, @"
            SELECT DISTINCT CAST([Division] AS NVARCHAR(500)) AS Value
            FROM dbo.EducationUnits
            WHERE [Division] IS NOT NULL
              AND LTRIM(RTRIM(CAST([Division] AS NVARCHAR(500)))) <> ''
            ORDER BY Value");

                result.Results = GetDistinctStringList(conn, @"
            SELECT DISTINCT CAST([CGPA] AS NVARCHAR(100)) AS Value
            FROM dbo.EducationUnits
            WHERE [CGPA] IS NOT NULL
              AND LTRIM(RTRIM(CAST([CGPA] AS NVARCHAR(100)))) <> ''
            ORDER BY Value");
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
                    if (reader[0] != DBNull.Value)
                    {
                        var value = reader[0].ToString().Trim();

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
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();

                int employeeId = 0;

                string employeeQuery = @"
            SELECT TOP 1 Id
            FROM dbo.Employees
            WHERE LTRIM(RTRIM(CAST(EmployeeId AS NVARCHAR(50)))) = LTRIM(RTRIM(@EmployeeCode))";

                using (SqlCommand empCmd = new SqlCommand(employeeQuery, conn))
                {
                    empCmd.Parameters.AddWithValue("@EmployeeCode", employeeCode);

                    object result = empCmd.ExecuteScalar();
                    if (result == null || result == DBNull.Value)
                    {
                        throw new Exception("Employee code not found.");
                    }

                    employeeId = Convert.ToInt32(result);
                }

                string query;

                if (dto.Id == 0)
                {
                    query = @"
                INSERT INTO dbo.EducationUnits
                (EmployeeId, EducationId, [Group], Board, AcademicYear, AcademicInstitute, Division, CGPA)
                VALUES
                (@EmployeeId, @EducationId, @Group, @Board, @AcademicYear, @AcademicInstitute, @Division, @Result)";
                }
                else
                {
                    query = @"
                UPDATE dbo.EducationUnits
                SET
                    EducationId = @EducationId,
                    [Group] = @Group,
                    Board = @Board,
                    AcademicYear = @AcademicYear,
                    AcademicInstitute = @AcademicInstitute,
                    Division = @Division,
                    CGPA = @Result
                WHERE Id = @Id";
                }

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (dto.Id != 0)
                        cmd.Parameters.AddWithValue("@Id", dto.Id);

                    cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                    cmd.Parameters.AddWithValue("@EducationId", dto.EducationId);
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
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = "DELETE FROM dbo.EducationUnits WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }

            return true;
        }
    }
}