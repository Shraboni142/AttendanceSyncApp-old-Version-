using System.Collections.Generic;
using AttendanceSyncApp.Models.DTOs.Payroll;

namespace AttendanceSyncApp.Services.Interfaces.Payroll
{
    public interface IBonusTypeService
    {
        List<BonusTypeDto> GetAll();
        BonusTypeDto GetById(int id);
        bool Create(BonusTypeDto dto);
        bool Update(BonusTypeDto dto);
        bool Delete(int id);
    }
}