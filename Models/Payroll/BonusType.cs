using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceSyncApp.Models.Payroll
{
    [Table("BonusTypes")]
    public class BonusType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string BonusTypeName { get; set; }

        [Required]
        [StringLength(150)]
        public string remark { get; set; }
    }
}