using System;
using System.Collections.Generic;

namespace HRMS_CSharp.Models
{
    public partial class ProjectTask
    {
        public int Id { get; set; }
        public int ProId { get; set; }
        public string TaskTitle { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string CreateDate { get; set; } = null!;
        public string TaskType { get; set; } = null!;
        public string? Location { get; set; }
        public string Status { get; set; } = null!;
        public string ApproveStatus { get; set; } = null!;
    }
}
