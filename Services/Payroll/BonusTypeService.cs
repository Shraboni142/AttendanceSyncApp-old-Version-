using System.Collections.Generic;
using System.Linq;
using AttendanceSyncApp.Models.DTOs.Payroll;
using AttendanceSyncApp.Models.Payroll;
using AttendanceSyncApp.Services.Interfaces.Payroll;

namespace AttendanceSyncApp.Services.Payroll
{
    public class BonusTypeService : IBonusTypeService
    {
        public List<BonusTypeDto> GetAll()
        {
            using (var db = new PayrollDbContext())
            {
                return db.BonusTypes
                    .OrderBy(x => x.Id)
                    .Select(x => new BonusTypeDto
                    {
                        Id = x.Id,
                        BonusTypeName = x.BonusTypeName,
                        remark = x.remark
                    })
                    .ToList();
            }
        }

        public BonusTypeDto GetById(int id)
        {
            using (var db = new PayrollDbContext())
            {
                var entity = db.BonusTypes.FirstOrDefault(x => x.Id == id);
                if (entity == null) return null;

                return new BonusTypeDto
                {
                    Id = entity.Id,
                    BonusTypeName = entity.BonusTypeName,
                    remark = entity.remark
                };
            }
        }

        public bool Create(BonusTypeDto dto)
        {
            using (var db = new PayrollDbContext())
            {
                var entity = new BonusType
                {
                    BonusTypeName = dto.BonusTypeName,
                    remark = dto.remark
                };

                db.BonusTypes.Add(entity);
                return db.SaveChanges() > 0;
            }
        }

        public bool Update(BonusTypeDto dto)
        {
            using (var db = new PayrollDbContext())
            {
                var entity = db.BonusTypes.FirstOrDefault(x => x.Id == dto.Id);
                if (entity == null) return false;

                entity.BonusTypeName = dto.BonusTypeName;
                entity.remark = dto.remark;

                return db.SaveChanges() > 0;
            }
        }

        public bool Delete(int id)
        {
            using (var db = new PayrollDbContext())
            {
                var entity = db.BonusTypes.FirstOrDefault(x => x.Id == id);
                if (entity == null) return false;

                db.BonusTypes.Remove(entity);
                return db.SaveChanges() > 0;
            }
        }
    }
}