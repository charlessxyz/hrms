using System;
using System.Collections.Generic;

namespace HRMS_CSharp.Models
{
    public partial class Addition
    {
        public string AddiId { get; set; } = null!;
        public string? Basic { get; set; }
        public string? Conveyance { get; set; }
        public string? HouseRent { get; set; }
        public string? Medical { get; set; }
        public string? SalaryId { get; set; }
    }
}
