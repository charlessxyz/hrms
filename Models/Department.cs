using System;
using System.Collections.Generic;

namespace HRMS_CSharp.Models
{
    public partial class Department
    {
        public int Id { get; set; }
        public string DepName { get; set; } = null!;

        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
