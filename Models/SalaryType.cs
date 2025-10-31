using System;
using System.Collections.Generic;

namespace HRMS_CSharp.Models
{
    public partial class SalaryType
    {
        public int Id { get; set; }
        public string? SalaryType1 { get; set; }
        public string? CreateDate { get; set; }

        public virtual ICollection<EmpSalary> EmpSalaries { get; set; } = new List<EmpSalary>();
        public virtual ICollection<PaySalary> PaySalaries { get; set; } = new List<PaySalary>();
    }
}
