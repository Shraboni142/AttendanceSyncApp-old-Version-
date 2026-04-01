namespace AttendanceSyncApp.Models.DTOs.EmployeeInformation
{
    public class EmployeeInfoGeneralDto
    {
        public int Id { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public string MobileNo { get; set; }
        public decimal? BasicSalary { get; set; }
        public string DateOfBirth { get; set; }
        public string DesignationName { get; set; }
        public string DepartmentName { get; set; }
        public string BranchName { get; set; }
    }
}