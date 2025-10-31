using System;
using System.Collections.Generic;

namespace HRMS_CSharp.Models
{
    public partial class Project
    {
        public int Id { get; set; }
        public string ProName { get; set; } = null!;
        public DateTime? ProStartDate { get; set; }
        public DateTime? ProEndDate { get; set; }
        public string ProStatus { get; set; } = null!;
        public string? ProDescription { get; set; }
        public string? ProSummary { get; set; }
        public int? Progress { get; set; }
    }
}
