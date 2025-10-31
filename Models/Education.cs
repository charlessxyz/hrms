using System;
using System.Collections.Generic;

namespace HRMS_CSharp.Models
{
    public partial class Education
    {
        public string Id { get; set; } = null!;
        public string? EduType { get; set; }
        public string? EmpId { get; set; }
        public string? Institute { get; set; }
        public string? Result { get; set; }
        public string? Year { get; set; }
    }
}
