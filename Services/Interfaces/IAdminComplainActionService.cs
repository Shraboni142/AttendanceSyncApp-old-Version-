using AttendanceSyncApp.Models.DTOs.ComplainAction;
using System.Collections.Generic;

namespace AttendanceSyncApp.Services.Interfaces
{
    public interface IAdminComplainActionService
    {
        bool SaveComplainAction(ComplainActionCreateDto dto);
        List<ComplainActionListDto> GetAllComplainActions();
        ComplainActionCreateDto GetComplainActionById(int id);
        bool UpdateComplainAction(ComplainActionCreateDto dto);
        bool DeleteComplainAction(int id);
        bool UpdateComplainReviewStatus(int id, string reviewStatus);
    }
}