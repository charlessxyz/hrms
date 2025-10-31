using System;
using System.Collections.Generic;

namespace HRMS_CSharp.Models
{
    public partial class LeaveType
    {
        public int TypeId { get; set; }
        public string Name { get; set; } = null!;
        public string LeaveDay { get; set; } = null!;
        public sbyte Status { get; set; }
    }
}
