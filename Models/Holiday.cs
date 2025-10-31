using System;
using System.Collections.Generic;

namespace HRMS_CSharp.Models
{
    public partial class Holiday
    {
        public int Id { get; set; }
        public string? HolidayName { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public string? NumberOfDays { get; set; }
        public string? Year { get; set; }
    }
}
