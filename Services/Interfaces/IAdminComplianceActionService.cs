using AttendanceSyncApp.Models.DTOs.ComplianceAction;
using System.Collections.Generic;

namespace AttendanceSyncApp.Services.Interfaces
{
    public interface IAdminComplianceActionService
    {
        bool SaveComplianceAction(ComplianceActionCreateDto dto);
        bool UpdateComplianceAction(ComplianceActionCreateDto dto);
        bool DeleteComplianceAction(int id);

        ComplianceActionCreateDto GetComplianceActionById(int id);
        List<ComplianceActionListDto> GetAllComplianceActions();
    }
}