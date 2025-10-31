using System;
using System.Collections.Generic;

namespace HRMS_CSharp.Models
{
    public partial class EarnedLeave
    {
        public int Id { get; set; }
        public string? EmId { get; set; }
        public string? PresentDate { get; set; }
        public string? Hour { get; set; }
        public string? Status { get; set; }
    }
}
