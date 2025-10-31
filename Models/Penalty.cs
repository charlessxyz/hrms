using System;
using System.Collections.Generic;

namespace HRMS_CSharp.Models
{
    public partial class Penalty
    {
        public int Id { get; set; }
        public string? PenaltyName { get; set; }
        public string? PenaltyAmount { get; set; }
    }
}
