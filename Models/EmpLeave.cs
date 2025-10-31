using System;
using System.Collections.Generic;

namespace HRMS_CSharp.Models
{
    public partial class EmpLeave
    {
        public int Id { get; set; }
        public string? EmId { get; set; }
        public int Typeid { get; set; }
        public string? LeaveType { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string? LeaveDuration { get; set; }
        public string? ApplyDate { get; set; }
        public string? Reason { get; set; }
        public string LeaveStatus { get; set; } = null!;
    }
}
