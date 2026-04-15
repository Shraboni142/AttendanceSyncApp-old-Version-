using System.ComponentModel.DataAnnotations;

namespace AttendanceSyncApp.Models.DTOs.Payroll
{
    public class BonusTypeDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100)]
        public string BonusTypeName { get; set; }

        [Required(ErrorMessage = "Remarks is required.")]
        [StringLength(150)]
        public string remark { get; set; }
    }
}