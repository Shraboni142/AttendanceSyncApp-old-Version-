using AttendanceSyncApp.Models.DTOs.ComplianceAction;
using System.Collections.Generic;

namespace AttendanceSyncApp.Services.Interfaces
{
    public interface IAdminComplianceActionService
    {
        bool SaveComplianceAction(ComplianceActionCreateDto dto);
        List<ComplianceActionListDto> GetAllComplianceActions();
        ComplianceActionCreateDto GetComplianceActionById(int id);
        bool UpdateComplianceAction(ComplianceActionCreateDto dto);
        bool DeleteComplianceAction(int id);
        bool UpdateComplianceReviewStatus(int id, string reviewStatus);
    }
}