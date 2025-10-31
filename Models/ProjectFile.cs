using System;

namespace HRMS_CSharp.Models
{
    public partial class ProjectFile
    {
        public int Id { get; set; }
        public int ProId { get; set; }
        public string FileDetails { get; set; } = null!;
        public string FileUrl { get; set; } = null!;
        public int AssignedTo { get; set; }
    }
}
