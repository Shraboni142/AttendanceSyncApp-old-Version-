using System.Collections.Generic;

namespace AttendanceSyncApp.Models.DTOs.EmployeeInformation
{
    public class EducationFieldDropdownsDto
    {
        public List<string> Groups { get; set; }
        public List<string> Boards { get; set; }
        public List<string> AcademicYears { get; set; }
        public List<string> AcademicInstitutes { get; set; }
        public List<string> Divisions { get; set; }
        public List<string> Results { get; set; }
    }
}