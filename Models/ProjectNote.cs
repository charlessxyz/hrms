using System;

namespace HRMS_CSharp.Models
{
    public partial class ProjectNote
    {
        public int Id { get; set; }
        public int ProId { get; set; }
        public string Details { get; set; } = null!;
        public int AssignTo { get; set; }
    }
}
