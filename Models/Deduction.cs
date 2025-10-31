using System;
using System.Collections.Generic;

namespace HRMS_CSharp.Models
{
    public partial class Deduction
    {
        public string DeId { get; set; } = null!;
        public string? Bima { get; set; }
        public string? Others { get; set; }
        public string? ProvidentFund { get; set; }
        public string? SalaryId { get; set; }
        public string? Tax { get; set; }
    }
}
