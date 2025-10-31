using System;
using System.Collections.Generic;

namespace HRMS_CSharp.Models
{
    public partial class AssignLeave
    {
        public string Id { get; set; } = null!;
        public string? AppId { get; set; }
        public string? Day { get; set; }
        public int? EmpId { get; set; }
        public string? Hour { get; set; }
        public string? TotalDay { get; set; }
        public int? TypeId { get; set; }
        public string? Dateyear { get; set; }
    }
}
