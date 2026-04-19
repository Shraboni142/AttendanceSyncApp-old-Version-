using System;
using System.Web;

namespace AttendanceSyncApp.Models.DTOs.ComplainAction
{
    public class ComplainActionCreateDto
    {
        public int Id { get; set; }

        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }

        public string ReviewStatus { get; set; }

        public string OffenceType { get; set; }
        public string OffenceDetails { get; set; }

        public string ComplainActionType { get; set; }
        public string ComplainActionDetails { get; set; }

        public DateTime DateOfNotice { get; set; }
        public DateTime? EarlyWithdrawalDate { get; set; }

        public string AttachmentFileName { get; set; }
        public string AttachmentFilePath { get; set; }

        public string CreatedBy { get; set; }

        public HttpPostedFileBase AttachmentFile { get; set; }
    }
}