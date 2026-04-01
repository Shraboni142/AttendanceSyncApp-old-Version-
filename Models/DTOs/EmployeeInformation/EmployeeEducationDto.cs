namespace AttendanceSyncApp.Models.DTOs.EmployeeInformation
{
    public class EmployeeEducationDto
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int EducationId { get; set; }
        public string EducationName { get; set; }
        public string Group { get; set; }
        public string Board { get; set; }
        public string AcademicYear { get; set; }
        public string AcademicInstitute { get; set; }
        public string Division { get; set; }
        public string Result { get; set; }
    }
}