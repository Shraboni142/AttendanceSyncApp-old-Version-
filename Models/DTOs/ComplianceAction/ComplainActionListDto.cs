namespace AttendanceSyncApp.Models.DTOs.ComplainAction
{
	public class ComplainActionListDto
	{
		public int Id { get; set; }

		public string EmployeeCode { get; set; }
		public string EmployeeName { get; set; }

		public string OffenceType { get; set; }
		public string OffenceDetails { get; set; }

		public string ReviewStatus { get; set; }

		public string ComplainActionType { get; set; }
		public string ComplainActionDetails { get; set; }
	}
}