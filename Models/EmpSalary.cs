using System;
using System.Collections.Generic;

namespace HRMS_CSharp.Models
{
    public partial class EmpSalary
    {
        public int Id { get; set; }
        public string? EmpId { get; set; }
        public int TypeId { get; set; }
        public string? Total { get; set; }

        public virtual Employee? Emp { get; set; }
        public virtual SalaryType Type { get; set; } = null!;
    }
}
