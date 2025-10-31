using System;
using System.Collections.Generic;

namespace HRMS_CSharp.Models
{
    public partial class FieldVisit
    {
        public int Id { get; set; }
        public string ProjectId { get; set; } = null!;
        public string? EmpId { get; set; }
        public string FieldLocation { get; set; } = null!;
        public string? StartDate { get; set; }
        public string ApproxEndDate { get; set; } = null!;
        public string? TotalDays { get; set; }
        public string Notes { get; set; } = null!;
        public string ActualReturnDate { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string AttendanceUpdated { get; set; } = null!;
    }
}
