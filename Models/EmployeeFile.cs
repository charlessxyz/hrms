using System;
using System.Collections.Generic;

namespace HRMS_CSharp.Models
{
    public partial class EmployeeFile
    {
        public string Id { get; set; } = null!;
        public string? EmId { get; set; }
        public string? FileTitle { get; set; }
        public string? FileUrl { get; set; }
    }
}
