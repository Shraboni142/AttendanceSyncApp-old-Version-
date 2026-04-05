using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text;

namespace AttendanceSyncApp.Models
{
    [Table("Emps")]
    public class Emp
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public List<EmpEducation> Educations { get; set; }
    }

    [Table("EmpAddresses")]
    public class EmpAddress
    {
        [Key]
        public int Id { get; set; }

        public int EmpId { get; set; }
        [ForeignKey("EmpId")]
        public Emp Emp { get; set; }

        public string Address { get; set; }
    }


    [Table("EmpEducations")]
    public class EmpEducation
    {
        [Key]
        public int Id { get; set; }

        public int EmpId { get; set; }
        [ForeignKey("EmpId")]
        public Emp Emp { get; set; }

        public string EducationName { get; set; }
    }
}