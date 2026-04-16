using System;
using System.Web;

namespace AttendanceSyncApp.Models.DTOs.ComplianceAction
{
    public class ComplianceActionCreateDto
    {
        public int? EmployeeId { get; set; }
        public int Id { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }

        public string OffenceType { get; set; }
        public string OffenceDetails { get; set; }

        public string ComplianceActionType { get; set; }
        public string ComplianceActionDetails { get; set; }

        public DateTime DateOfNotice { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime? EarlyWithdrawalDate { get; set; }

        public string AttachmentFileName { get; set; }
        public string AttachmentFilePath { get; set; }

        public string CreatedBy { get; set; }
        public HttpPostedFileBase AttachmentFile { get; set; }
    }
}