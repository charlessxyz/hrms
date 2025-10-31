using System;
using System.Collections.Generic;

namespace HRMS_CSharp.Models
{
    public partial class EmpExperience
    {
        public string Id { get; set; } = null!;
        public string? EmpId { get; set; }
        public string? ExpComAddress { get; set; }
        public string? ExpComPosition { get; set; }
        public string? ExpCompany { get; set; }
        public string? ExpWorkduration { get; set; }
    }
}
