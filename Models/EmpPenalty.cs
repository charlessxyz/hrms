using System;
using System.Collections.Generic;

namespace HRMS_CSharp.Models
{
    public partial class EmpPenalty
    {
        public int Id { get; set; }
        public int EmpId { get; set; }
        public int PenaltyId { get; set; }
        public string? PenaltyDesc { get; set; }
    }
}
