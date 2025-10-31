using System;
using System.Collections.Generic;

namespace HRMS_CSharp.Models
{
    public partial class Notice
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? FileUrl { get; set; }
        public string? Date { get; set; }
    }
}
