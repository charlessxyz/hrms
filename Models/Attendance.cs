using System;
using System.Collections.Generic;

namespace HRMS_CSharp.Models
{
    public partial class Attendance
    {
        public int Id { get; set; }
        public string? EmpId { get; set; }
        public string? AttenDate { get; set; }
        public TimeSpan? SigninTime { get; set; }
        public TimeSpan? SignoutTime { get; set; }
        public string? WorkingHour { get; set; }
        public string Place { get; set; } = null!;
        public string? Absence { get; set; }
        public string? Overtime { get; set; }
        public string? Earnleave { get; set; }
        public string? Status { get; set; }
    }
}
