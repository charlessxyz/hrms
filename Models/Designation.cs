using System;
using System.Collections.Generic;

namespace HRMS_CSharp.Models
{
    public partial class Designation
    {
        public int Id { get; set; }
        public string DesName { get; set; } = null!;

        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
